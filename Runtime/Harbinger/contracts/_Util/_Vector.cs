using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Types()
        {
            AddContract(new("random_inSphere", typeof(Vector3), function: static exe => Random.insideUnitSphere));

            AddContract(new("random_onSphere", typeof(Vector3), function: static exe => Random.onUnitSphere));

            AddContract(new("set", typeof(Vector3),
                args: static exe =>
                {
                    if (!exe.reader.TryReadString_matches_out(out string item, true, default, matches: new string[] { "x", "y", "z", }))
                        exe.reader.Stderr($"please specify x or y or z.");
                    else
                    {
                        exe.args.Add(item);

                        if (exe.pipe_previous == null)
                            if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                                exe.reader.Stderr($"expected float expression.");
                            else
                                exe.arg_0 = expr;

                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_vect))
                            exe.args.Add(expr_vect);
                        else
                            exe.reader.Stderr($"expected vector.");
                    }
                },
                routine: static exe =>
                {
                    string item = (string)exe.args[0];
                    ExpressionExecutor expr_vect = (ExpressionExecutor)exe.args[1];

                    using var routine_vect = expr_vect.EExecute();
                    using var routine_float = exe.arg_0.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: _ =>
                        {
                            Vector3 v = (Vector3)routine_vect.Current.output;
                            float f = routine_float.Current.output switch { int _i => _i, float _f => _f, _ => 0, };

                            switch (item)
                            {
                                case "x":
                                    v.x = f;
                                    break;
                                case "y":
                                    v.y = f;
                                    break;
                                case "z":
                                    v.z = f;
                                    break;
                            }
                            return v;
                        },
                        routine_vect,
                        routine_float);
                }));

            AddContract(new("vector3", typeof(Vector3),
                args: static exe =>
                {
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_x))
                        exe.reader.Stderr($"expected x.");
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_y))
                        exe.reader.Stderr($"expected y.");
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_z))
                        exe.reader.Stderr($"expected z.");
                    else
                    {
                        exe.args.Add(expr_x);
                        exe.args.Add(expr_y);
                        exe.args.Add(expr_z);
                    }
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr_x = (ExpressionExecutor)exe.args[0];
                    ExpressionExecutor expr_y = (ExpressionExecutor)exe.args[1];
                    ExpressionExecutor expr_z = (ExpressionExecutor)exe.args[2];

                    using var routine_x = expr_x.EExecute();
                    using var routine_y = expr_y.EExecute();
                    using var routine_z = expr_z.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: _ =>
                        {
                            float x = routine_x.Current.output switch { int _int => _int, float _float => _float, _ => 0 };
                            float y = routine_y.Current.output switch { int _int => _int, float _float => _float, _ => 0 };
                            float z = routine_z.Current.output switch { int _int => _int, float _float => _float, _ => 0 };
                            return new Vector3(x, y, z);
                        },
                        routine_x,
                        routine_y,
                        routine_z);
                }
                ));
        }
    }
}