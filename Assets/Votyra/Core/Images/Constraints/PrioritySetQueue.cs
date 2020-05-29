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
            this._valueEqualityComparer = valueEqualityComparer;
            this._queue = new LinkedList<PrioritisedValue>();
        }

        public PrioritySetQueue(IEnumerable<TValue> initialValue, IEqualityComparer<TValue> valueEqualityComparer, Func<TValue, TPriority> func, IComparer<TPriority> create)
        {
            this._valueEqualityComparer = valueEqualityComparer;
            this._queue = new LinkedList<PrioritisedValue>();
            this.Reset(create);

            foreach (var value in initialValue)
            {
                this.Add(value, func(value));
            }
        }

        public int Count => this._queue.Count;

        public PrioritisedValue GetFirst()
        {
            var cellWithValue = this._queue.First;
            this.RemoveNode(cellWithValue);

            return cellWithValue.Value;
        }

        public void Add(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var addded = false;
            var node = this._queue.First;
            while (node != null)
            {
                var isOldToRemove = this._valueEqualityComparer.Equals(node.Value.Value, newCellToCheck);
                var isNewToBeAddedBeforeCurrent = (this._priorityOrderComparer.Compare(node.Value.Priority, newCellToCheckValue) > 0) && !addded; //node.Value.Value < newCellToCheckValue && !addded;

                if (isOldToRemove && isNewToBeAddedBeforeCurrent)
                {
                    node.Value = this.GetValue(newCellToCheck, newCellToCheckValue);
                    addded = true;
                    break;
                }

                if (isOldToRemove)
                {
                    this.RemoveNode(node);
                }
                else if (isNewToBeAddedBeforeCurrent)
                {
                    this._queue.AddBefore(node, this.GetNode(newCellToCheck, newCellToCheckValue));
                    addded = true;
                }

                node = node.Next;
            }

            if (!addded)
            {
                this._queue.AddLast(this.GetNode(newCellToCheck, newCellToCheckValue));
            }
        }

        public void Reset(IComparer<TPriority> priorityOrderComparer)
        {
            this._priorityOrderComparer = priorityOrderComparer;
            var node = this._queue.First;
            while (node != null)
            {
                this._emptyNodes.ReturnRaw(node);
                node = node.Next;
            }

            this._queue.Clear();
        }

        private void RemoveNode(LinkedListNode<PrioritisedValue> node)
        {
            this._emptyNodes.ReturnRaw(node);
            this._queue.Remove(node);
        }

        private LinkedListNode<PrioritisedValue> GetNode(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var value = this.GetValue(newCellToCheck, newCellToCheckValue);
            var node = this._emptyNodes.GetRaw();
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
                this.Value = pos;
                this.Priority = value;
            }
        }
    }
}
