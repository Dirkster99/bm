﻿namespace SuggestLib
{
    using Interfaces;
    using SuggestLib.SuggestSource;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Implements a text based control that updates a list of suggestions
    /// when user updates a given text based path -> TextChangedEvent is raised.
    /// 
    /// This control uses <see cref="ISuggestSource"/> and HierarchyHelper
    /// to suggest entries in a seperate popup as the user types.
    /// </summary>
    public class SuggestBox : SuggestBoxBase
    {
        #region fields
        /// <summary>
        /// Implements the backing store for the <see cref="SuggestSources"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SuggestSourcesProperty = DependencyProperty.Register(
            "SuggestSources", typeof(IEnumerable<ISuggestSource>),
            typeof(SuggestBox), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing property of the <see cref="RootItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootItemProperty =
            DependencyProperty.Register("RootItem", typeof(object),
                typeof(SuggestBox), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store for the <see cref="PathValidation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathValidationProperty =
            DependencyProperty.Register("PathValidation", typeof(ValidationRule),
                typeof(SuggestBox), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store for the <see cref="IsDeferredScrolling"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDeferredScrollingProperty =
            DependencyProperty.Register("IsDeferredScrolling",
                typeof(bool), typeof(SuggestBox), new PropertyMetadata(false));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static SuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox),
                new FrameworkPropertyMetadata(typeof(SuggestBox)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SuggestBox()
        {
            IsVisibleChanged += SuggestBox_IsVisibleChanged;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets a list of data sources that can be queries in order to
        /// display suggested entries to the user.
        /// </summary>
        public IEnumerable<ISuggestSource> SuggestSources
        {
            get { return (IEnumerable<ISuggestSource>)GetValue(SuggestSourcesProperty); }
            set { SetValue(SuggestSourcesProperty, value); }
        }

        /// <summary>
        /// Gets/sets a dependency property that holds an object that represents the current
        /// location. This location object is also handed down to the SuggestedSources
        /// object to make finding the next list of suggestions a simple matter of retrieving
        /// the children of the current rootitems object collection.
        /// 
        /// This property can be assigned by the client application (eg. Breadcrumb) and be
        /// updated throughout the browsing with suggestions.
        /// </summary>
        public object RootItem
        {
            get { return GetValue(RootItemProperty); }
            set { SetValue(RootItemProperty, value); }
        }

        /// <summary>
        /// Gets/sets a <see cref="ValidationRule"/> that must be present to show a
        /// validation error (red rectangle around textbox) if user entered invalid data.
        /// </summary>
        public ValidationRule PathValidation
        {
            get { return (ValidationRule)GetValue(PathValidationProperty); }
            set { SetValue(PathValidationProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the scrollbar <see cref="ListBox"/> inside the suggestions PopUp
        /// control is directly linked to scrolling the content or not (is deferred).
        /// 
        /// This property is handled by the control itself and should not be used via binding.
        /// </summary>
        public bool IsDeferredScrolling
        {
            get { return (bool)GetValue(IsDeferredScrollingProperty); }
            set { SetValue(IsDeferredScrollingProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method executes when the <see cref="SuggestBoxBase.EnableSuggestions"/> dependency property
        /// has changed its value.
        /// 
        /// Overwrite this method if you want to consume changes of this property.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnEnableSuggestionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnEnableSuggestionChanged(e);

            if (((bool)e.NewValue) == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when the visibility of the control is changed to query for
        /// suggestions if this was enabled...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue) == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when new text is entered in the textbox portion of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            QueryForSuggestions();
        }

        private void QueryForSuggestions()
        {
            // Text change is likely to be from property change so we ignore it
            // if control is invisible or suggestions are currently not requested
            if (Visibility != Visibility.Visible || EnableSuggestions == false)
                return;

            try
            {
                var suggestSources = this.SuggestSources;
                string input = this.Text;
                object location = this.RootItem;
                IsHintVisible = String.IsNullOrEmpty(input);

                if (IsEnabled == true)
                {
                    _suggestionIsConsumed = false;
                }

                if (IsEnabled && suggestSources != null)
                {
                    Task.Run(async () =>
                    {
                        List<Task<ISuggestResult>> tasks = new List<Task<ISuggestResult>>();
                        foreach (var item in suggestSources)
                        {
                            // Query suggestion source for suggestions on this input
                            tasks.Add(item.SuggestAsync(location, input));
                        }

                        await Task.WhenAll(tasks);

                        // Consolidate all results into 1 result object
                        ISuggestResult AllResults = new SuggestResult(
                            tasks.SelectMany(tsk => tsk.Result.Suggestions).Distinct().ToList());

                        // See if a suggestion source invalidates the input if there are no results
                        if (AllResults.Suggestions.Count == 0)
                        {
                            var validPaths = tasks.Where(tsk => tsk.Result.ValidPath == false);
                            if (validPaths.Any())
                                AllResults.ValidPath = false;  // No SuggestionSource could not validate input
                        }

                        return AllResults;
                    }
                    ).ContinueWith(
                        (pTask) =>
                        {
                            if (pTask.IsFaulted == false)
                            {
                                // Determine whether deferred scrolling makes any sense or not
                                if (pTask.Result.Suggestions.Count > 4096)
                                    IsDeferredScrolling = true;
                                else
                                    IsDeferredScrolling = false;

                                this.SetValue(SuggestionsProperty, pTask.Result.Suggestions);

                                if (pTask.Result.ValidPath == true)
                                    MarkInvalidInputSuggestBox(false);
                                else
                                    MarkInvalidInputSuggestBox(true, "Path is not valid.");
                            }

                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch
            {
                // logger.Error(exp);
            }
        }

        /// <summary>
        /// Sets or clears a validation error on the SuggestBox
        /// to indicate invalid input to the user.
        /// </summary>
        /// <param name="markError">True: Shows a red validation error rectangle around the SuggestBox
        /// (<paramref name="msg"/> should also be set).
        /// False: Clears previously set validation errors around the Text property of the SuggestBox.
        /// </param>
        /// <param name="msg">Error message (eg.: "invalid input") is set on the binding expression if
        /// <paramref name="markError"/> is true.</param>
        private void MarkInvalidInputSuggestBox(bool markError, string msg = null)
        {
            if (markError == true)
            {
                // Show a red validation error rectangle around SuggestionBox
                // if validation rule dependency property is available
                if (PathValidation != null)
                {
                    var bindingExpr = this.GetBindingExpression(TextBox.TextProperty);
                    if (bindingExpr != null)
                    {
                        Validation.MarkInvalid(bindingExpr,
                                new ValidationError(PathValidation, bindingExpr, msg, null));
                    }
                }
            }
            else
            {
                // Clear validation error in case it was previously set switching from Text to TreeView
                var bindingExpr = this.GetBindingExpression(TextBox.TextProperty);
                if (bindingExpr != null)
                    Validation.ClearInvalid(bindingExpr);
            }
        }
        #endregion
    }
}
