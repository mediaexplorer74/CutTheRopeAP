using System.Collections.Generic;
using GameManager.iframework.core;
using GameManager.iframework.helpers;
using GameManager.ios;

namespace GameManager.iframework.sfe
{
    internal class ConstraintedPoint : MaterialPoint
    {
        public Vector prevPos;

        public Vector pin;

        public List<Constraint> constraints;

        public override void dealloc()
        {
            constraints = null;
            base.dealloc();
        }

        public override NSObject init()
        {
            if (base.init() != null)
            {
                prevPos = vect(2.1474836E+09f, 2.1474836E+09f);
                pin = vect(-1f, -1f);
                constraints = new List<Constraint>();
            }
            return this;
        }

        public virtual void addConstraintwithRestLengthofType(ConstraintedPoint c, float r, Constraint.CONSTRAINT t)
        {
            Constraint constraint = new Constraint();
            constraint.init();
            constraint.cp = c;
            constraint.restLength = r;
            constraint.type = t;
            constraints.Add(constraint);
        }

        public virtual void removeConstraint(ConstraintedPoint o)
        {
            for (int i = 0; i < constraints.Count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint.cp == o)
                {
                    constraints.RemoveAt(i);
                    break;
                }
            }
        }

        public virtual void removeConstraints()
        {
            constraints = new List<Constraint>();
        }

        public virtual void changeConstraintFromTo(ConstraintedPoint o, ConstraintedPoint n)
        {
            int count = constraints.Count;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint != null && constraint.cp == o)
                {
                    constraint.cp = n;
                    break;
                }
            }
        }

        public virtual void changeConstraintFromTowithRestLength(ConstraintedPoint o, ConstraintedPoint n, float l)
        {
            int count = constraints.Count;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint != null && constraint.cp == o)
                {
                    constraint.cp = n;
                    constraint.restLength = l;
                    break;
                }
            }
        }

        public virtual void changeRestLengthToFor(float l, ConstraintedPoint n)
        {
            int count = constraints.Count;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint != null && constraint.cp == n)
                {
                    constraint.restLength = l;
                    break;
                }
            }
        }

        public virtual bool hasConstraintTo(ConstraintedPoint p)
        {
            int count = constraints.Count;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint != null && constraint.cp == p)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual float restLengthFor(ConstraintedPoint n)
        {
            int count = constraints.Count;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = constraints[i];
                if (constraint != null && constraint.cp == n)
                {
                    return constraint.restLength;
                }
            }
            return -1f;
        }

        public override void resetAll()
        {
            base.resetAll();
            prevPos = vect(2.1474836E+09f, 2.1474836E+09f);
            removeConstraints();
        }

        public override void update(float delta)
        {
            update(delta, 1f);
        }

        public virtual void update(float delta, float koeff)
        {
            totalForce = vectZero;
            if (!disableGravity)
            {
                if (!vectEqual(globalGravity, vectZero))
                {
                    totalForce = vectAdd(totalForce, vectMult(globalGravity, weight));
                }
                else
                {
                    totalForce = vectAdd(totalForce, gravity);
                }
            }
            if (highestForceIndex != -1)
            {
                for (int i = 0; i <= highestForceIndex; i++)
                {
                    totalForce = vectAdd(totalForce, forces[i]);
                }
            }
            totalForce = vectMult(totalForce, invWeight);
            a = vectMult(totalForce, (double)delta / 1.0 * (double)delta / 1.0);
            if (prevPos.x == 2.1474836E+09f)
            {
                prevPos = pos;
            }
            posDelta.x = pos.x - prevPos.x + a.x;
            posDelta.y = pos.y - prevPos.y + a.y;
            v = vectMult(posDelta, (float)(1.0 / (double)delta));
            prevPos = pos;
            pos = vectAdd(pos, posDelta);
        }

        public static void satisfyConstraints(ConstraintedPoint p)
        {
            if (p.pin.x != -1f)
            {
                p.pos = p.pin;
                return;
            }
            int count = p.constraints.Count;
            Vector vector = vectZero;
            Vector vector2 = vectZero;
            for (int i = 0; i < count; i++)
            {
                Constraint constraint = p.constraints[i];
                vector.x = constraint.cp.pos.x - p.pos.x;
                vector.y = constraint.cp.pos.y - p.pos.y;
                if (vector.x == 0f && vector.y == 0f)
                {
                    vector = vect(1f, 1f);
                }
                float num = vectLength(vector);
                float restLength = constraint.restLength;
                switch (constraint.type)
                {
                case Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN:
                    if (num <= restLength)
                    {
                        continue;
                    }
                    break;
                case Constraint.CONSTRAINT.CONSTRAINT_NOT_LESS_THAN:
                    if (num >= restLength)
                    {
                        continue;
                    }
                    break;
                }
                vector2 = vector;
                float num2 = constraint.cp.invWeight;
                float num3 = ((num > 1f) ? num : 1f);
                float num4 = (num - restLength) / (num3 * (p.invWeight + num2));
                float num5 = p.invWeight * num4;
                vector.x *= num5;
                vector.y *= num5;
                num5 = num2 * num4;
                vector2.x *= num5;
                vector2.y *= num5;
                p.pos.x += vector.x;
                p.pos.y += vector.y;
                if (constraint.cp.pin.x == -1f)
                {
                    constraint.cp.pos = vectSub(constraint.cp.pos, vector2);
                }
            }
        }

        public static void qcpupdate(ConstraintedPoint p, float delta, float koeff)
        {
            p.totalForce = vectZero;
            if (!p.disableGravity)
            {
                if (!vectEqual(globalGravity, vectZero))
                {
                    p.totalForce = vectAdd(p.totalForce, vectMult(globalGravity, p.weight));
                }
                else
                {
                    p.totalForce = vectAdd(p.totalForce, p.gravity);
                }
            }
            if (p.highestForceIndex != -1)
            {
                for (int i = 0; i <= p.highestForceIndex; i++)
                {
                    p.totalForce = vectAdd(p.totalForce, p.forces[i]);
                }
            }
            p.totalForce = vectMult(p.totalForce, p.invWeight);
            p.a = vectMult(p.totalForce, (float)((double)delta / 1.0 * 0.01600000075995922 * (double)koeff));
            if (p.prevPos.x == 2.1474836E+09f)
            {
                p.prevPos = p.pos;
            }
            p.posDelta.x = p.pos.x - p.prevPos.x + p.a.x;
            p.posDelta.y = p.pos.y - p.prevPos.y + p.a.y;
            p.v = vectMult(p.posDelta, (float)(1.0 / (double)delta));
            p.prevPos = p.pos;
            p.pos = vectAdd(p.pos, p.posDelta);
        }
    }
}
