using System;
using System.Collections.Generic;
using Votyra.Core.Pooling;

namespace Votyra.Core.Images.Constraints
{
    public class PrioritySetQueue<TValue, TPriority>
    {
        private readonly Pool<LinkedListNode<PrioritisedValue>> emptyNodes = new Pool<LinkedListNode<PrioritisedValue>>(() => new LinkedListNode<PrioritisedValue>(default));
        private readonly LinkedList<PrioritisedValue> queue;
        private readonly IEqualityComparer<TValue> valueEqualityComparer;
        private IComparer<TPriority> priorityOrderComparer;

        public PrioritySetQueue(IEqualityComparer<TValue> valueEqualityComparer)
        {
            this.valueEqualityComparer = valueEqualityComparer;
            this.queue = new LinkedList<PrioritisedValue>();
        }

        public PrioritySetQueue(IEnumerable<TValue> initialValue, IEqualityComparer<TValue> valueEqualityComparer, Func<TValue, TPriority> func, IComparer<TPriority> create)
        {
            this.valueEqualityComparer = valueEqualityComparer;
            this.queue = new LinkedList<PrioritisedValue>();
            this.Reset(create);

            foreach (var value in initialValue)
            {
                this.Add(value, func(value));
            }
        }

        public int Count => this.queue.Count;

        public PrioritisedValue GetFirst()
        {
            var cellWithValue = this.queue.First;
            this.RemoveNode(cellWithValue);

            return cellWithValue.Value;
        }

        public void Add(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var addded = false;
            var node = this.queue.First;
            while (node != null)
            {
                var isOldToRemove = this.valueEqualityComparer.Equals(node.Value.Value, newCellToCheck);
                var isNewToBeAddedBeforeCurrent = (this.priorityOrderComparer.Compare(node.Value.Priority, newCellToCheckValue) > 0) && !addded; // node.Value.Value < newCellToCheckValue && !addded;

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
                    this.queue.AddBefore(node, this.GetNode(newCellToCheck, newCellToCheckValue));
                    addded = true;
                }

                node = node.Next;
            }

            if (!addded)
            {
                this.queue.AddLast(this.GetNode(newCellToCheck, newCellToCheckValue));
            }
        }

        public void Reset(IComparer<TPriority> priorityOrderComparer)
        {
            this.priorityOrderComparer = priorityOrderComparer;
            var node = this.queue.First;
            while (node != null)
            {
                this.emptyNodes.ReturnRaw(node);
                node = node.Next;
            }

            this.queue.Clear();
        }

        private void RemoveNode(LinkedListNode<PrioritisedValue> node)
        {
            this.emptyNodes.ReturnRaw(node);
            this.queue.Remove(node);
        }

        private LinkedListNode<PrioritisedValue> GetNode(TValue newCellToCheck, TPriority newCellToCheckValue)
        {
            var value = this.GetValue(newCellToCheck, newCellToCheckValue);
            var node = this.emptyNodes.GetRaw();
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
