#define P1
// #define P2
// #define P3

using _13_Scheduling;

#if P1
P1 p1 = new P1();
p1.Test();
p1.Test2();
#endif

#if P2
P2 p2 = new P2();
p2.Test();
p2.Test2();
p2.Test3();
#endif

#if P3
P3 p3 = new P3();
IEnumerable<IEnumerable<P3.Matrix>> collections = new List<IEnumerable<P3.Matrix>>();
float degrees = 45.0f;
p3.RotateMatrices(collections, degrees);
#endif