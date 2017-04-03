/// <summary>
/// The Networking namespace.
/// </summary>
namespace Networking
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// models a line. original intent was to attach the guid to identify lines. This didn't work however due to the xaml
    /// not accepting this as a ui element. adding list of start, end points to nodel model instead. NO LONGER NEEDED.
    /// </summary>
    public class LineModel 
    {
        public LineGeometry Line;
        public Guid Uid;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineModel"/> class.
        /// </summary>
        public LineModel()
        {
            Uid = new Guid();
            Line = new LineGeometry();
        }

        /// <summary>
        /// Initializes a new instance of the linmodel class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LineModel(Point start, Point end)
        {
            Uid = new Guid();
            Line = new LineGeometry(start, end);
        }
    }
}
