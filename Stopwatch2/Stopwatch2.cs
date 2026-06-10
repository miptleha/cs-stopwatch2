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
            _root = new Stopwatch2Task("Total time", null);
        }

        ///<summary>Stops a stopwatch</summary>
        public void Stop()
        {
            if (_root != null)
                _root.Stop();
        }

        ///<summary>Starts nested stopwatch for the task</summary>
        ///<param name="name">Task name</param>
        public Stopwatch2Task Start(string name)
        {
            if (_simpleMode)
                return Stopwatch2Task.Dummy;

            var t = new Stopwatch2Task(name, _root);
            return t;
        }

        ///<summary>Stopwatch results in text form</summary>
        ///<param name="opt">Display options</param>
        public string Results(Stopwatch2Options opt = null)
        {
            if (_root == null)
                return null;

            _root.Stop();
            opt = opt ?? new Stopwatch2Options();
            return _root.Results(0, _root.Elapsed, opt.HideRoot, opt.HidePercent, opt.HideTime, opt.HideCount, opt.MsMode);
        }
    }

    public class Stopwatch2Options
    {
        ///<summary>Hide root</summary>
        public bool HideRoot = false;
        ///<summary>Hide percent</summary>
        public bool HidePercent = false;
        ///<summary>Hide time</summary>
        public bool HideTime = false;
        ///<summary>Hide count</summary>
        public bool HideCount = false;
        ///<summary>Execution time in miliseconds</summary>
        public bool MsMode = false;
    }

    ///<summary>Stopwatch for one task</summary>
    public class Stopwatch2Task : IDisposable
    {
        Dictionary<string, Stopwatch2Task> _children = new Dictionary<string, Stopwatch2Task>();
        Stopwatch2Task _parent;
        string _name;
        Stopwatch _sw;
        TimeSpan _time = TimeSpan.Zero;
        int _cnt = 0;
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
            Start();
        }

        ///<summary>Starts a stopwatch, can only be called once</summary>
        private void Start()
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
                _cnt++;
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
            return t;
        }

        ///<summary>Stopwatch results in text form</summary>
        public string Results(int level, TimeSpan parentElapsed, bool hideSelf, bool hidePercent, bool hideTime, bool hideCount, bool msMode)
        {
            StringBuilder result = new StringBuilder();
            if (!hideSelf)
            {
                int percent = (int)Math.Round(_time.TotalMilliseconds / parentElapsed.TotalMilliseconds * 100);
                string str = "";
                if (!hideTime)
                    str += msMode ? Math.Round(_time.TotalMilliseconds).ToString() + "ms" : _time.ToString();
                if (!hidePercent)
                    str += (str.Length == 0 ? "" : " ") + percent + "%";
                if (!hideCount)
                    str += (str.Length == 0 ? "" : " ") + _cnt;
                string sep = str.Length == 0 ? "" : ": "; 
                result.Append(new string(' ', level) + _name + sep + str);
            }

            var sorted = _children
                .OrderByDescending(kvp => kvp.Value._time)
                .Select(kvp => kvp.Value);
            foreach (var item in sorted)
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
