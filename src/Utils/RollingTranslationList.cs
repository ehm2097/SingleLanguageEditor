// MIT License

// Copyright (c) 2024 Andrei Vatasescu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;

namespace Avat.SingleLanguageEditor.Utils;

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