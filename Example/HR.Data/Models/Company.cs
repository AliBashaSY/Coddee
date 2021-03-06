//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Coddee tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Coddee;
using System;
using System.Runtime.Serialization;


namespace HR.Data.Models
{
    
    public class Company : IUniqueObject<int>, IEquatable<IUniqueObject<int>>, IEquatable<Company>
    {
		public System.Int32 Id { get; set; }
		public System.String Name { get; set; }

        public int BranchCount { get; set; }
        public int EmployeeCount { get; set; }

        public string Detail => $"Branches: {BranchCount}    Employees: {EmployeeCount}";

		[IgnoreDataMember]
		public System.Int32 GetKey =>Id;

        public override bool Equals(object obj)
        {
			var other = obj as IUniqueObject<System.Int32>;
            if (other != null)
            {
                return this.Equals(other);
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return this.GetKey.GetHashCode();
        }
        public virtual bool Equals(IUniqueObject<int> other)
        {
            if (((this.GetKey != -1) 
                        && (other.GetKey != -1)))
            {
                return (this.GetKey == other.GetKey);
            }
            else
            {
                return object.ReferenceEquals(this, other);
            }
        }
        public virtual bool Equals(Company other)
        {
            if (((this.GetKey != -1) 
                        && (other.GetKey != -1)))
            {
                return (this.GetKey == other.GetKey);
            }
            else
            {
                return object.ReferenceEquals(this, other);
            }
        }
    }
}
