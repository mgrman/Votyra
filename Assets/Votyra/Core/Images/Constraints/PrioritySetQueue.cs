using System;
using System.Collections.Generic;
using Votyra.Core.Pooling;

namespace Votyra.Core.Images.Constraints
{
    public class PrioritySetQueue<TValue, TPriority>
    {
        private readonly Pool<LinkedListNode<PrioritisedValue>> _emptyNodes = new Pool<LinkedListNode<PrioritisedValue>>(() => new LinkedListNode<PrioritisedValue>(default));
        private readonly LinkedList<PrioritisedValue> _queue;
        private readonly IEqualityComparer<TValue> _valueEqualityComparer;
        private IComparer<TPriority> _priorityOrderComparer;

        public PrioritySetQueue(IEqualityComparer<TValue> valueEqualityComparer)
        {
            _valueEqualityComparer = valueEqualityComparer;
            _queue = new LinkedList<PrioritisedValue>();
        }

        public PrioritySetQueue(IEnumerable<TValue> initialValue, IEqualityComparer<TValue> valueEqualityComparer, Func<TValue, TPriority> func, IComparer<TPriority> create)
        {
            _valueEqualityComparer = valueEqualityComparer;
            _queue = new LinkedList<PrioritisedValue>();
            Reset(create);

            foreach (var value in initialValue)
            {
                Add(value, func(value));
            }
        }

        public int Count => _queue.Count;

        public PrioritisedValue GetFirst()
        {
            var cellWithValue = _queue.First;
            RemoveNode(cellWithValue);

            return cellWithValue.Value;
        }

        public void Add(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var addded = false;
            var node = _queue.First;
            while (node != null)
            {
                var isOldToRemove = _valueEqualityComparer.Equals(node.Value.Value, newCellToCheck);
                var isNewToBeAddedBeforeCurrent = _priorityOrderComparer.Compare(node.Value.Priority, newCellToCheckValue) > 0 && !addded; //node.Value.Value < newCellToCheckValue && !addded;

                if (isOldToRemove && isNewToBeAddedBeforeCurrent)
                {
                    node.Value = GetValue(newCellToCheck, newCellToCheckValue);
                    addded = true;
                    break;
                }

                if (isOldToRemove)
                {
                    RemoveNode(node);
                }
                else if (isNewToBeAddedBeforeCurrent)
                {
                    _queue.AddBefore(node, GetNode(newCellToCheck, newCellToCheckValue));
                    addded = true;
                }

                node = node.Next;
            }

            if (!addded)
            {
                _queue.AddLast(GetNode(newCellToCheck, newCellToCheckValue));
            }
        }

        public void Reset(IComparer<TPriority> priorityOrderComparer)
        {
            _priorityOrderComparer = priorityOrderComparer;
            var node = _queue.First;
            while (node != null)
            {
                _emptyNodes.ReturnRaw(node);
                node = node.Next;
            }

            _queue.Clear();
        }

        private void RemoveNode(LinkedListNode<PrioritisedValue> node)
        {
            _emptyNodes.ReturnRaw(node);
            _queue.Remove(node);
        }

        private LinkedListNode<PrioritisedValue> GetNode(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var value = GetValue(newCellToCheck, newCellToCheckValue);
            var node = _emptyNodes.GetRaw();
            node.Value = value;
            return node;
        }

        private PrioritisedValue GetValue(TValue newCellToCheck, TPriority newCellToCheckValue) => new PrioritisedValue(newCellToCheck, newCellToCheckValue);

        public struct PrioritisedValue
        {
            public readonly TValue Value;
            public readonly TPriority Priority;

            public PrioritisedValue(TValue pos, TPriority value)
            {
                Value = pos;
                Priority = value;
            }
        }
    }
}
