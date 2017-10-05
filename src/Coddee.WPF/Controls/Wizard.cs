﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Controls
{
    [DefaultProperty("Steps")]
    [ContentProperty("Steps")]
    public class Wizard : Control
    {
        static Wizard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Wizard), new FrameworkPropertyMetadata(typeof(Wizard)));
        }



        public static readonly DependencyProperty StepsProperty = DependencyProperty.Register(
                                                        "Steps",
                                                        typeof(ObservableCollection<WizardStep>),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ObservableCollection<WizardStep>), StepsValueChanged));

        public static readonly DependencyPropertyKey FirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                                "FirstStep",
                                                                                                                typeof(WizardStep),
                                                                                                                typeof(WizardStep),
                                                                                                                new PropertyMetadata(default(WizardStep)));



        public static readonly DependencyProperty FirstStepProperty = FirstStepPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey LastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                               "LastStep",
                                                                                                               typeof(WizardStep),
                                                                                                               typeof(WizardStep),
                                                                                                               new PropertyMetadata(default(WizardStep)));

        public static readonly DependencyProperty LastStepProperty = LastStepPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
                                                        "CurrentStep",
                                                        typeof(WizardStep),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(WizardStep)));


        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(
                                                        "SaveCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
                                                        "CancelCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        public ICommand CancelCommand
        {
            get { return (ICommand) GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public ICommand SaveCommand
        {
            get { return (ICommand) GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public WizardStep CurrentStep
        {
            get { return (WizardStep)GetValue(CurrentStepProperty); }
            set
            {
                CurrentStep?.SetCurrent(false);
                SetValue(CurrentStepProperty, value);
                RefreshCommands();
                value?.SetCompleted(true);
                value?.SetCurrent(true);
            }
        }

        public WizardStep LastStep
        {
            get { return (WizardStep)GetValue(LastStepProperty); }
            protected set { SetValue(LastStepPropertyKey, value); }
        }


        public WizardStep FirstStep
        {
            get { return (WizardStep)GetValue(FirstStepProperty); }
            protected set { SetValue(FirstStepPropertyKey, value); }
        }

        private static void StepsValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Wizard wizard)
            {
                wizard.RefreshSteps();
            }
        }

        public Wizard()
        {
            Steps = new ObservableCollection<WizardStep>();
            Steps.CollectionChanged += (sender, args) =>
            {
                RefreshSteps();
            };

            NextCommand = new ReactiveCommand<Wizard>(this, Next)
                .ObserveProperty(e => e.CurrentStep, ValidateCanGoNext);

            BackCommand = new ReactiveCommand<Wizard>(this, Back)
                .ObserveProperty(e => e.CurrentStep, ValidateCanGoBack);
            
        }

        public IReactiveCommand NextCommand { get; }
        public IReactiveCommand BackCommand { get; }


        private void RefreshCommands()
        {
            NextCommand?.UpdateCanExecute();
            BackCommand?.UpdateCanExecute();
        }
        private bool ValidateCanGoNext(object obj)
        {
            if (obj is WizardStep step)
                return !step.IsLastStep;
            return false;
        }
        private bool ValidateCanGoBack(object obj)
        {
            if (obj is WizardStep step)
                return !step.IsFirstStep;
            return false;
        }

        private void Back()
        {
            if (!CurrentStep.IsFirstStep)
            {
                CurrentStep.SetCompleted(false);
                CurrentStep = Steps.First(e => e.Index == CurrentStep.Index - 1);
            }
        }

        private void Next()
        {
            if (!CurrentStep.IsLastStep)
            {
                CurrentStep = Steps.First(e => e.Index == CurrentStep.Index + 1);
            }
        }

        public ObservableCollection<WizardStep> Steps
        {
            get { return (ObservableCollection<WizardStep>)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        public void RefreshSteps()
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].SetIndex(i);
            }

            FirstStep?.ClearFisrtAndLast();
            LastStep?.ClearFisrtAndLast();

            FirstStep = Steps.FirstOrDefault();
            LastStep = Steps.LastOrDefault();

            FirstStep?.SetFirst();
            LastStep?.SetLast();


            CurrentStep = FirstStep;

        }
    }

    public class WizardStep : DependencyObject
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                        "ViewModel",
                                                        typeof(IPresentableViewModel),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(IPresentableViewModel), ViewModelSet));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                                                        "Title",
                                                        typeof(string),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(string)));

        public static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "Index",
                                                        typeof(int),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(int)));

        public static readonly DependencyProperty IndexProperty = IndexPropertyKey.DependencyProperty;


        public static readonly DependencyPropertyKey IsFirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsFirstStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsFirstStepProperty = IsFirstStepPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey IsLastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsLastStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsLastStepProperty = IsLastStepPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                                                        "Content",
                                                        typeof(object),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(object)));

        public static readonly DependencyPropertyKey IsCompletedPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCompleted",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsCompletedProperty = IsCompletedPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey IsCurrentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCurrent",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsCurrentProperty = IsCurrentPropertyKey.DependencyProperty;

        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            protected set { SetValue(IsCurrentPropertyKey, value); }
        }

        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            protected set { SetValue(IsCompletedPropertyKey, value); }
        }
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public bool IsLastStep
        {
            get { return (bool)GetValue(IsLastStepProperty); }
            protected set { SetValue(IsLastStepPropertyKey, value); }
        }
        public bool IsFirstStep
        {
            get { return (bool)GetValue(IsFirstStepProperty); }
            protected set { SetValue(IsFirstStepPropertyKey, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            protected set { SetValue(IndexPropertyKey, value); }
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IPresentableViewModel ViewModel
        {
            get { return (IPresentableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void ViewModelSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WizardStep wizardStep)
            {
                wizardStep.UpdateContent();
            }
        }

        private void UpdateContent()
        {
            Content = ViewModel.GetView();
        }

        internal void SetIndex(int index)
        {
            Index = index;
        }

        public void ClearFisrtAndLast()
        {
            IsFirstStep = false;
            IsLastStep = false;
        }

        public void SetFirst()
        {
            IsFirstStep = true;
        }

        public void SetLast()
        {
            IsLastStep = true;
        }

        public void SetCompleted(bool newValue)
        {
            IsCompleted = newValue;
        }
        public void SetCurrent(bool newValue)
        {
            IsCurrent = newValue;
        }
    }

    public class WizardStepTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FirstStepTemplate { get; set; }
        public DataTemplate LastStepTemplate { get; set; }
        public DataTemplate StepTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WizardStep step)
            {
                if (step.IsFirstStep)
                    return FirstStepTemplate;
                if (step.IsLastStep)
                    return LastStepTemplate;

                return StepTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}