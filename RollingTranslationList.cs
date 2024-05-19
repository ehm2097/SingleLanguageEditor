using System.Collections;
using Accessibility;

namespace Avat.SingleLanguageEditor;

internal class RollingTranslationList : IEnumerable<object>
{
    public RollingTranslationList(LinkedListNode<object> first,
        LinkedListNode<object>? current, LinkedListNode<object> last)
        => (_first, _current, _last) = (first, current ?? first, last);
    
    public IEnumerator<object> GetEnumerator() => new InternalEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly LinkedListNode<object> _first;
    private readonly LinkedListNode<object> _current;
    private readonly LinkedListNode<object> _last;

    private class InternalEnumerator : IEnumerator<object>
    {
        public InternalEnumerator(RollingTranslationList list)
            => (_first, _initial, _last) = (list._first, list._current, list._last);

        public void Dispose() { }

        public object Current => _current != null ? _current.Value : _initial;

        public void Reset() => _current = null;

        public bool MoveNext()
        {
            if(_current == null)
            {
                _current = _initial;
                return true;
            }

            _current = _current == _last ? _first : _current.Next;
            return (_current != _initial);
        }

        private readonly LinkedListNode<object> _first;
        private readonly LinkedListNode<object> _initial;
        private LinkedListNode<object>? _current = null;
        private readonly LinkedListNode<object> _last;
    }
}