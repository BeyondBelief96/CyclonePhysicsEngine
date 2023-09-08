using Assets.Cyclone.RigidBodies;
using System;

namespace Assets.Cyclone.CollisionDetection.BVH
{
    /// <summary>
    /// Holds the bodies that might be in contact.
    /// </summary>
    public struct PotentialContact
    {
        public RigidBody[] Bodies;
    }

    /// <summary>
    /// A class for nodes in a bounding volume hierarchy. This class uses
    /// a binary tree to store the bounding volumes.
    /// </summary>
    public class BVHNode<TBoundingVolume>
    {
        #region Fields

        private BVHNode<TBoundingVolume> _thisNode;

        #endregion

        #region Ctor

        public BVHNode(BVHNode<TBoundingVolume> bVHNode, BoundingVolume volume, RigidBody body)
        {
            Volume = volume;
            Body = body;
            _thisNode = bVHNode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the child nodes of this node. Max of 2 children per node.
        /// </summary>
        public BVHNode<TBoundingVolume>[] Children { get; set; }

        /// <summary>
        /// Holds a single bounding volume encompassing all
        /// of the descendants of this node.
        /// </summary>
        public BoundingVolume Volume { get; set; }

        /// <summary>
        /// holds the node immediately above us in the tree.
        /// </summary>
        public BVHNode<TBoundingVolume> Parent { get; set; }

        /// <summary>
        /// Holds the rigid body at this node of the hierarchy.
        /// Only the leaf nodes can have a rigid body defined.
        /// Note that it is possible to rewrite the algorithms in this
        /// class to handle objects at all levels of the hierarchy,
        /// but the code provided ignores this vector unless firstChild is null.
        /// </summary>
        public RigidBody Body { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Checks whether this node is at the bottom of the hierarchy.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return (Body != null);
        }

        /// <summary>
        /// Checks the potential contacts from this node downwards in the hierarchy,
        /// writing them to the given array (up to the given limit). Returns the
        /// number of potential contacts it found.
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public uint GetPotentialContacts(PotentialContact[] contacts, uint limit)
        {
            //Early out if we don't have the room for contacts or if were a leaf node.
            if(IsLeaf() || limit == 0) return 0;

            //Get the potential contacts of one of our children with the other.
            return Children[0].GetPotentialContactsWith(Children[1], contacts, limit);
        }

        public bool Overlaps(BVHNode<TBoundingVolume> other)
        {
            return Volume.Overlaps(other.Volume);
        }

        /// <summary>
        /// Inserts the given rigid body, with the given bounding volume,
        /// into the hierarchy. This may involve the creation of further
        /// bounding volume nodes.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="volume"></param>
        public void Insert(RigidBody body, BoundingVolume newVolume)
        {
            //If we are a leaf, then the only option is to spawn two
            //new children and place the new body in one.
            if(IsLeaf())
            {
                //Child one is a copy of us.
                Children[0] = new BVHNode<TBoundingVolume>(this, Volume, body);

                //Child two holds the new body.
                Children[1] = new BVHNode<TBoundingVolume>(this, Volume, body);

                //And we now loose the body (we're no longer a leaf).
                Body = null;

                //We need to recalculate our bounding volume.
                RecalculateBoundingVolume();
            }
            //Otherwise, we need to work out which child gets to keep
            //the inserted body. We give it to whoever would grow the
            //least to incorporate it.
            else
            {
                if (Children[0].Volume.GetGrowth(newVolume) <
                        Children[1].Volume.GetGrowth(newVolume))
                {
                    Children[0].Insert(body, newVolume);
                }
                else
                {
                    Children[1].Insert(body, newVolume);
                }
            }
        }

        /// <summary>
        /// Deletes this node, removing it first from the hierarchy, along
        /// with its associated rigid body and child nodes. This method
        /// deletes the node and all its children (but obviously not the rigid bodies).
        /// This also has the effect of deleting the sibling of this node, and
        /// changing the parent node so that it contains the data currently
        /// in the system. Finally, it forces the hierarchy above the current
        /// node to reconsider its bounding volume.
        /// </summary>
        public void RemoveNode()
        {
            //If we don't have a parent then we ignore the sibling processing.
            if(Parent != null)
            {
                //Find our sibling.
                BVHNode<TBoundingVolume> sibling;
                if (Parent.Children[0] == this) sibling = Parent.Children[1];
                else sibling = Parent.Children[0];

                //Write its data to our parent.
                Parent.Volume = sibling.Volume;
                Parent.Body = sibling.Body;
                Parent.Children[0] = sibling.Children[0];
                Parent.Children[1] = sibling.Children[1];

                //Delete the sibling (we blank its parent and
                //children to avoid processing/deleting them).
                sibling.Parent = null;
                sibling.Body = null;
                sibling.Children[0] = null;
                sibling.Children[1] = null;

                //Recalculate the parent's bounding volume.
                Parent.RecalculateBoundingVolume();
            }

            //Delete our children (again we remove their
            //parent data so we don't try to process their siblings
            //as they are deleted).
            if (Children[0] != null)
                Children[0].Parent = null;
            if (Children[1] != null)
                Children[1].Parent = null;
        }

        #endregion

        #region Private Functions

        private uint GetPotentialContactsWith(BVHNode<TBoundingVolume> other,
            PotentialContact[] contacts, uint limit)
        {
            //Early out if we don't overlap or if we have no room 
            //to report contacts.
            if (!Overlaps(other) || limit == 0) return 0;

            //If we're both at leaf nodes, then we have a potential contact.
            if (IsLeaf() && other.IsLeaf())
            {
                contacts[limit].Bodies[0] = Body;
                contacts[limit].Bodies[1] = other.Body;
                return 1;
            }

            //Determine which node to descend into. If either is
            //a leaf, then we descend the other. If both are branches,
            //then we use the one with the largest size.
            if (other.IsLeaf() || (!IsLeaf() && Volume.Size >= other.Volume.Size))
            {
                //Recurse into ourself.
                uint count = Children[0].GetPotentialContactsWith(other, contacts, limit);
                //Check we have enough slots to do the other side too.
                if (limit > count)
                {
                    return count + Children[1].GetPotentialContactsWith(other, contacts, limit - count);
                }
                else
                {
                    return count;
                }
            }
            else
            {
                //Recurse into the other node.
                uint count = GetPotentialContactsWith(other.Children[0], contacts, limit);
                //Check we have enough slots to do the other side too.
                if (limit > count)
                {
                    return count + GetPotentialContactsWith(other.Children[1], contacts, limit - count);
                }
                else
                {
                    return count;
                }
            }
        }

        private void RecalculateBoundingVolume()
        {
            Volume = Volume.RecalculateVolume(Children[0], Children[1]);
        }

        #endregion
    }
}
