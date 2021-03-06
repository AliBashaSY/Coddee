//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Coddee tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Coddee.Data;
using HR.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.Data.Repositories
{

    [Coddee.Data.RepositoryAttribute(typeof(IEmployeeRepository))]
    public class EmployeeRepository : CRUDFileRepositoryBase<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository() :
                base("Employee")
        {
        }

        private RepositoryDataFile<int, List<EmployeeJob>> _employeeJobs;

        protected override void CreateDataFiles()
        {
            base.CreateDataFiles();
            _employeeJobs = CreateDataFile<int, List<EmployeeJob>>("EmployeeJobs");
        }

        public async Task<EmployeeJob> InsertEmployeeJob(EmployeeJob item)
        {
            var employeeJobsCollection = await _employeeJobs.GetCollection();
            List<EmployeeJob> employeeJobs;
            if (!employeeJobsCollection.TryGetValue(item.EmployeeId, out employeeJobs))
            {
                employeeJobs = new List<EmployeeJob> { item };
                employeeJobsCollection.TryAdd(item.EmployeeId, employeeJobs);
            }
            else
            {
                employeeJobs.Add(item);
            }

            await _employeeJobs.UpdateFile();
            EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(Coddee.OperationType.Add, item));
            return item;
        }

        public async Task<IEnumerable<EmployeeJob>> GetEmployeeJobsByEmployee(int employeeId)
        {
            var employeeJobsCollection = await _employeeJobs.GetCollection();
            List<EmployeeJob> result = null;
            employeeJobsCollection.TryGetValue(employeeId, out result);
            return result.AsEnumerable();
        }

        public Task<IEnumerable<Employee>> GetItemsWithDetailes()
        {
            return GetItems();
        }

        public Task<Employee> GetItemWithDetailes(int employeeId)
        {
            return this[employeeId];
        }

        public async Task DeleteEmployeeJob(EmployeeJob employeeJob)
        {
            var collection = await _employeeJobs.GetCollection();
            var jobs = collection[employeeJob.EmployeeId];
            var job = jobs.FirstOrDefault(e => e.BranchId == employeeJob.BranchId && e.DepartmentId == employeeJob.DepartmentId && e.JobId == employeeJob.JobId);
            if (job != null)
                jobs.Remove(job);
            EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(Coddee.OperationType.Delete, employeeJob));
        }

        public event EventHandler<RepositoryChangeEventArgs<EmployeeJob>> EmployeeJobsChanged;

        public async Task<IEnumerable<Employee>> GetItemsWithDetailesByBranch(int branchId)
        {
            var collection = await _employeeJobs.GetCollection();
            var branchEmployees = collection.Where(e => e.Value.Any(j => j.BranchId == branchId));
            return (await GetCollection()).Where(e => branchEmployees.Any(b => b.Key == e.Key)).Select(e=>e.Value).AsEnumerable();
        }
    }
}
