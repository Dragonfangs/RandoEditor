
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class LogLayer
    {
        public LogLayer()
        {
        }

        public LogLayer(string message)
        {
            Message = message;
        }

        public LogLayer(string message, IEnumerable<string> children)
        {
            Message = message;
            Children = children.Select(child => new LogLayer(child)).ToList();
        }

        public LogLayer(string message, IEnumerable<LogLayer> children)
        {
            Message = message;
            Children = children.ToList();
        }

        public void New()
        {
            New(string.Empty);
        }

        public void New(string message)
        {
            Message = message;
            Children = new List<LogLayer>();
        }

        public LogLayer AddChild(string message)
        {
            return AddChild(new LogLayer(message));
        }

        public LogLayer AddChild(string message, IEnumerable<string> children)
        {
            return AddChild(new LogLayer(message, children));
        }

        public LogLayer AddChild(string message, IEnumerable<LogLayer> children)
        {
            return AddChild(new LogLayer(message, children));
        }

        public LogLayer AddChild(LogLayer layer)
        {
            Children.Add(layer);
            return layer;
        }

        public string Message { get; set; }

        public List<LogLayer> Children { get; set; } = new List<LogLayer>();
    }
}
