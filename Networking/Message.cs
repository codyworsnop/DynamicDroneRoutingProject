/// <summary>
/// Message.cs
/// </summary>
namespace Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The intended purpose of this class is to model the data to be sent through the mesh network. 
    /// </summary>
    class Message
    {
        private byte type { get; set; } // 1: text, 2: image, 3: video; text only implemented now.
        private string message { get; set; }
    }
}