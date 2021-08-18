using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualBasic;

// https://stackoverflow.com/questions/22285866/why-relaycommand

namespace CONTROLBPA
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///    ''' Creates a new command that can always execute.
        ///    ''' </summary>
        ///    ''' <param name="execute">The execution logic</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        ///    ''' Creates a new command.
        ///    ''' </summary>
        ///    ''' <param name="execute">The execution logic</param>
        ///    ''' <param name="canExecute">The execution status logic</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if ((execute == null))
                throw new ArgumentNullException("execute");
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return ((this._canExecute == null) || this._canExecute.Invoke(parameter));
        }

        public void Execute(object parameter)
        {
            this._execute.Invoke(parameter);
        }
    }

}
