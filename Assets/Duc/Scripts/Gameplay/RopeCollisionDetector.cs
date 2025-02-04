using Obi;
using System.Collections.Generic;
using UnityEngine;

namespace Duc.PoppyTangle
{
    public class RopeCollisionDetector : MonoBehaviour
    {
        public static RopeCollisionDetector instance;

        public ObiSolver solver;
        public Dictionary<ObiRope, int> ropeContacts = new Dictionary<ObiRope, int>();

        public int contactCount;
        ObiSolver.ObiCollisionEventArgs frame;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (solver == null)
            {
                solver = GetComponent<ObiSolver>();
            }
        }

        void Start()
        {
            solver.OnParticleCollision += Solver_OnParticleCollision;
            //solver.OnCollision += Solver_OnCollision;
        }

        void OnDestroy()
        {
            solver.OnParticleCollision -= Solver_OnParticleCollision;
            //solver.OnCollision -= Solver_OnCollision;
        }

        void Solver_OnParticleCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
        {
            frame = e;
            ropeContacts.Clear();
            foreach (var contact in e.contacts)
            {
                // this one is an actual collision:
                //if (contact.distance < 0.01)
                //{
                // retrieve the offset and size of the simplex in the solver.simplices array:
                int simplexStart = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyB, out int simplexSizeB);

                // starting at simplexStart, iterate over all particles in the simplex:
                for (int i = 0; i < simplexSizeB; ++i)
                {
                    int particleIndex = solver.simplices[simplexStart + i];
                    ObiSolver.ParticleInActor pa = solver.particleToActor[particleIndex];
                    if (pa != null)
                    {
                        ObiRope obiRope = pa.actor as ObiRope;
                        if (ropeContacts.ContainsKey(obiRope))
                        {
                            ropeContacts[obiRope]++;
                        }
                        else
                        {
                            ropeContacts.Add(obiRope, 1);
                        }
                    }
                }

                simplexStart = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyA, out int simplexSizeA);
                for (int i = 0; i < simplexSizeA; ++i)
                {
                    int particleIndex = solver.simplices[simplexStart + i];
                    ObiSolver.ParticleInActor pa = solver.particleToActor[particleIndex];
                    if (pa != null)
                    {
                        ObiRope obiRope = pa.actor as ObiRope;
                        if (ropeContacts.ContainsKey(obiRope))
                        {
                            ropeContacts[obiRope]++;
                        }
                        else
                        {
                            ropeContacts.Add(obiRope, 1);
                        }
                    }
                }
                //}
            }

            foreach (var rope in GameplayController.instance.ropes)
            {
                if (rope != null && rope.obiRope != null)
                {
                    if (!ropeContacts.ContainsKey(rope.obiRope))
                    {
                        if (!rope.isMerge)
                        {
                            rope.Merge();
                        }
                    }
                }
            }
        }

//#if UNITY_EDITOR
//        void OnDrawGizmos()
//        {
//            if (solver == null || frame == null || frame.contacts == null) return;

//            Gizmos.matrix = solver.transform.localToWorldMatrix;

//            contactCount = frame.contacts.Count;

//            /*for (int i = 0; i < frame.contacts.Count; ++i)
//            {
//                var contact = frame.contacts.Data[i];

//                //if (contact.distance > 0.001f) continue;

//                Gizmos.color = (contact.distance <= 0) ? Color.red : Color.green;

//                //Gizmos.color = new Color(((i * 100) % 255) / 255.0f, ((i * 50) % 255) / 255.0f, ((i * 20) % 255) / 255.0f);

//                Vector3 point = frame.contacts.Data[i].pointB;

//                Gizmos.DrawSphere(point, 0.01f);

//                Gizmos.DrawRay(point, contact.normal * contact.distance);

//                Gizmos.color = Color.cyan;
//                Gizmos.DrawRay(point, contact.tangent * contact.tangentImpulse + contact.bitangent * contact.bitangentImpulse);

//            }*/

//            for (int i = 0; i < frame.contacts.Count; ++i)
//            {
//                var contact = frame.contacts.Data[i];

//                //if (contact.distance > 0.001f) continue;

//                Gizmos.color = (contact.distance <= 0) ? Color.red : Color.green;

//                //Gizmos.color = new Color(((i * 100) % 255) / 255.0f, ((i * 50) % 255) / 255.0f, ((i * 20) % 255) / 255.0f);

//                Vector3 point = Vector3.zero;//frame.contacts.Data[i].point;

//                int simplexStart = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyB, out int simplexSize);

//                float radius = 0;
//                for (int j = 0; j < simplexSize; ++j)
//                {
//                    point += (Vector3)solver.positions[solver.simplices[simplexStart + j]] * contact.pointB[j];
//                    radius += solver.principalRadii[solver.simplices[simplexStart + j]].x * contact.pointB[j];
//                }

//                Vector3 normal = contact.normal;

//                //Gizmos.DrawSphere(point + normal.normalized * frame.contacts[i].distance, 0.01f);

//                Gizmos.DrawSphere(point + normal * radius, 0.01f);

//                Gizmos.DrawRay(point + normal * radius, normal.normalized * contact.distance);
//            }
//        }
//#endif
    }
}