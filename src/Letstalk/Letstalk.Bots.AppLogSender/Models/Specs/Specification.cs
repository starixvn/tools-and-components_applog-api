using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.AppLogSender.Models.Specs
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T candidate);

        ISpecification<T> And(ISpecification<T> other);

        ISpecification<T> Or(ISpecification<T> other);

        ISpecification<T> Not();
    }

    public abstract class CompositeSpecification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfiedBy(T candidate);

        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }

        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
    }

    public class AndSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public override bool IsSatisfiedBy(T candidate)
        {
            return left.IsSatisfiedBy(candidate) && right.IsSatisfiedBy(candidate);
        }
    }

    public class OrSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public override bool IsSatisfiedBy(T candidate)
        {
            return left.IsSatisfiedBy(candidate) || right.IsSatisfiedBy(candidate);
        }
    }

    public class NotSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> other;

        public NotSpecification(ISpecification<T> other)
        {
            this.other = other;
        }

        public override bool IsSatisfiedBy(T candidate)
        {
            return !other.IsSatisfiedBy(candidate);
        }
    }
}