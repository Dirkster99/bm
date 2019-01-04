namespace BmLib.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// HotTrack is a re-styled Border that highlight itself when IsMouseOver,
    /// IsDragging or IsSelected
    /// </summary>
    [TemplatePart(Name = "PART_HotTrackGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Selected", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_Background", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_Highlight", Type = typeof(Rectangle))]
    [TemplateVisualState(Name = "State_Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "State_MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "State_MouseOverGrayed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "State_Dragging", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "State_Selected", GroupName = "CommonStates")]
    ////[TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    ////[TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class HotTrack : ContentControl
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
                DependencyProperty.Register("IsSelected", typeof(bool),
                typeof(HotTrack), new UIPropertyMetadata(false,
                        new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBorderBrushProperty =
                DependencyProperty.Register("SelectedBorderBrush", typeof(Brush),
                typeof(HotTrack), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Implements the backing store of the <see cref="BackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundBrushProperty =
                DependencyProperty.Register("BackgroundBrush", typeof(Brush),
                typeof(HotTrack), new UIPropertyMetadata(SystemColors.HotTrackBrush));

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrushProperty =
                DependencyProperty.Register("SelectedBrush", typeof(Brush),
                typeof(HotTrack), new UIPropertyMetadata(SystemColors.ActiveCaptionBrush));

        /// <summary>
        /// Implements the backing store of the <see cref="HighlightBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty =
                DependencyProperty.Register("HighlightBrush", typeof(Brush),
                typeof(HotTrack), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(117, 255, 255, 255))));

        /// <summary>
        /// Implements the backing store of the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
                DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                typeof(HotTrack), new UIPropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Implements the backing store of the <see cref="IsDragging"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
                DependencyProperty.Register("IsDragging", typeof(bool),
                typeof(HotTrack), new UIPropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="IsDraggingOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggingOverProperty =
                DependencyProperty.Register("IsDraggingOver", typeof(bool),
                typeof(HotTrack), new UIPropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="FillFullRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillFullRowProperty =
                DependencyProperty.Register("FillFullRow", typeof(bool),
                typeof(HotTrack), new UIPropertyMetadata(false));

        /// <summary>
        /// Implements the backing store of the <see cref="UseTransition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseTransitionProperty =
                DependencyProperty.Register("UseTransition", typeof(bool), typeof(HotTrack), new PropertyMetadata(true));

        /// <summary>
        /// Implements the backing store of the <see cref="ThreeDStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThreeDStyleProperty =
                DependencyProperty.Register("ThreeDStyle",
                    typeof(bool),
                    typeof(HotTrack),
                    new PropertyMetadata(false));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static HotTrack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotTrack),
                    new FrameworkPropertyMetadata(typeof(HotTrack)));
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets/sets whether this item is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Brush"/> that is applied on the border of this item
        /// when it is selected.
        /// </summary>
        public Brush SelectedBorderBrush
        {
            get { return (Brush)GetValue(SelectedBorderBrushProperty); }
            set { this.SetValue(SelectedBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Brush"/> that is applied on the background of this item.
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { this.SetValue(BackgroundBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Brush"/> that is applied when the item is selected.
        /// </summary>
        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { this.SetValue(SelectedBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Brush"/> that is applied when the item is highlighted.
        /// </summary>
        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { this.SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the corner radius of this controls boder.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets/sets wether the control is currently being dragged or not.
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { this.SetValue(IsDraggingProperty, value); }
        }

        /// <summary>
        /// Gets/sets wether the control is currently being dragged over or not.
        /// </summary>
        public bool IsDraggingOver
        {
            get { return (bool)GetValue(IsDraggingOverProperty); }
            set { this.SetValue(IsDraggingOverProperty, value); }
        }

        /// <summary>
        /// For TreeView, create a mirror to completely highlight the whole row.
        /// </summary>
        public bool FillFullRow
        {
            get { return (bool)GetValue(FillFullRowProperty); }
            set { this.SetValue(FillFullRowProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether or not to use transitions in animations (???)
        /// </summary>
        public bool UseTransition
        {
            get { return (bool)GetValue(UseTransitionProperty); }
            set { this.SetValue(UseTransitionProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether Hotrack should be styled with 3D emulated
        /// surface impression or not.
        /// </summary>
        public bool ThreeDStyle
        {
            get { return (bool)GetValue(ThreeDStyleProperty); }
            set { this.SetValue(ThreeDStyleProperty, value); }
        }
        #endregion properties

        #region Methods
        /// <summary>
        /// Is called when a control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateStates(false);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseEnter attached event
        /// is raised on this element. Implement this method to add class handling for this
        /// event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseEventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.UpdateStates(this.UseTransition);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseLeave attached event
        /// is raised on this element. Implement this method to add class handling for this
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/>
        /// that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.UpdateStates(this.UseTransition);
        }

        private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            HotTrack ht = (HotTrack)sender;

            if (!(args.NewValue.Equals(args.OldValue)))
            {
                ht.UpdateStates(ht.UseTransition);
            }
        }

        /// <summary>
        /// Is executed when the corresponding visual state of the control is changing.
        /// Eg: If Mouse is over control, Mouse is leaving control, is enabled, is disabled, etc
        /// </summary>
        /// <param name="useTransition"></param>
        private void UpdateStates(bool useTransition)
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "State_Selected", useTransition);
            }
            else
            {
                if (this.IsDragging)
                {
                    VisualStateManager.GoToState(this, "State_Dragging", useTransition);
                }
                else
                {
                    if (this.IsDraggingOver)
                    {
                        VisualStateManager.GoToState(this, "State_DraggingOver", useTransition);
                    }
                    else
                    {
                        if (this.IsMouseOver)
                        {
                            if (this.IsEnabled)
                            {
                                VisualStateManager.GoToState(this, "State_MouseOver", useTransition);
                            }
                            else
                            {
                                VisualStateManager.GoToState(this, "State_MouseOverGrayed", useTransition);
                            }
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, "State_Normal", useTransition);
                        }
                    }
                }
            }

            ////if (IsFocused)
            ////    VisualStateManager.GoToState(this, "Focused", useTransition);
            ////else
            ////    VisualStateManager.GoToState(this, "Unfocused", useTransition);
        }
        #endregion
    }
}
