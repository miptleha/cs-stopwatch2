using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Misc
{
    ///<summary>Cumulative hierarchical stopwatch</summary>
    public class Stopwatch2
    {
        Stopwatch2Task _root;
        bool _simpleMode;

        ///<summary>Gets the total elapsed time</summary>
        public TimeSpan Elapsed { get { return _root.Elapsed; } }

        /// <param name="simpleMode">Disables stopwatches for nested tasks</param>
        public Stopwatch2(bool simpleMode = false)
        {
            _root = new Stopwatch2Task("Total time", null);
            _simpleMode = simpleMode;
        }

        ///<summary>Creates and starts a stopwatch</summary>
        /// <param name="simpleMode">Disables stopwatches for nested tasks</param>
        public static Stopwatch2 StartNew(bool simpleMode = false)
        {
            var sw = new Stopwatch2(simpleMode);
            sw.Start();
            return sw;
        }

        ///<summary>Starts a stopwatch</summary>
        public void Start()
        {
            _root.Start();
        }

        ///<summary>Stops a stopwatch</summary>
        public void Stop()
        {
            _root.Stop();
        }

        ///<summary>Starts nested stopwatch for the task</summary>
        ///<param name="name">Task name</param>
        public Stopwatch2Task Start(string name)
        {
            if (_simpleMode)
                return Stopwatch2Task.Dummy;

            var t = new Stopwatch2Task(name, _root);
            t.Start();
            return t;
        }

        ///<summary>Stopwatch results in text form</summary>
        ///<param name="hideRoot">Hide root</param>
        ///<param name="hidePercent">Hide percent</param>
        ///<param name="hideTime">Hide time</param>
        ///<param name="hideCount">Hide count</param>
        ///<param name="msMode">Execution time in miliseconds</param>
        public string Results(bool hideRoot = false, bool hidePercent = false, bool hideTime = false, bool hideCount = false, bool msMode = false)
        {
            _root.Stop();
            return _root.Results(0, _root.Elapsed, hideRoot, hidePercent, hideTime, hideCount, msMode);
        }
    }

    ///<summary>Stopwatch for one task</summary>
    public class Stopwatch2Task : IDisposable
    {
        Dictionary<string, Stopwatch2Task> _children = new Dictionary<string, Stopwatch2Task>();
        Stopwatch2Task _parent;
        string _name;
        Stopwatch _sw;
        TimeSpan _time = TimeSpan.Zero;
        int _cnt = 1;
        static object _lock = new object();
        static Stopwatch2Task _dummy = new Stopwatch2Task(null, null);

        ///<summary>Gets the total elapsed time</summary>
        public TimeSpan Elapsed { get { return _time; } }

        ///<summary>Disabled stopwatch</summary>
        public static Stopwatch2Task Dummy { get { return _dummy; } }

        public Stopwatch2Task(string name, Stopwatch2Task parent)
        {
            _name = name;
            _parent = parent;
            _sw = new Stopwatch();
        }

        ///<summary>Starts a stopwatch</summary>
        public void Start()
        {
            if (this == _dummy)
                return;

            _sw.Start();
        }

        ///<summary>Stops a stopwatch</summary>
        public void Stop()
        {
            if (this == _dummy)
                return;

            if (!_sw.IsRunning)
                return;

            _sw.Stop();

            lock (_lock)
            {
                _time += _sw.Elapsed;
                if (_parent != null)
                {
                    _parent.AddChild(this);
                }
            }
        }

        ///<summary>Saves child stopwatch info</summary>
        void AddChild(Stopwatch2Task t)
        {
            if (!_children.ContainsKey(t._name))
            {
                _children.Add(t._name, t);
            }
            else
            {
                var c = _children[t._name];
                c._time += t._time;
                c._cnt += t._cnt;
                foreach (var v in t._children.Values)
                {
                    c.AddChild(v);
                }
            }
        }

        ///<summary>Starts nested stopwatch</summary>
        ///<param name="name">Task name</param>
        public Stopwatch2Task Start(string name)
        {
            if (this == _dummy)
                return _dummy;

            var t = new Stopwatch2Task(name, this);
            t.Start();
            return t;
        }

        ///<summary>Stopwatch results in text form</summary>
        public string Results(int level, TimeSpan parentElapsed, bool hideSelf, bool hidePercent, bool hideTime, bool hideCount, bool msMode)
        {
            StringBuilder result = new StringBuilder();
            if (!hideSelf)
            {
                var items = new List<string>();
                if (!hideTime)
                {
                    items.Add(msMode ? Math.Round(_time.TotalMilliseconds).ToString() + "ms" : _time.ToString());
                }
                if (!hidePercent)
                {
                    int percent = (int)Math.Round(_time.TotalMilliseconds / parentElapsed.TotalMilliseconds * 100);
                    items.Add(percent + "%");
                }
                if (!hideCount)
                {
                    items.Add(_cnt.ToString());
                }
                string sep = items.Count == 0 ? "" : ": ";
                result.Append(new string(' ', level) + _name + sep + string.Join(" ", items));
            }

            var list = _children.Values.ToList();
            list = list.OrderByDescending(item => item._time).ToList();
            foreach (var item in list)
            {
                if (result.Length > 0)
                    result.Append("\n");
                result.Append(item.Results(hideSelf ? level : level + 1, _time, false, hidePercent, hideTime, hideCount, msMode));
            }

            return result.ToString();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
