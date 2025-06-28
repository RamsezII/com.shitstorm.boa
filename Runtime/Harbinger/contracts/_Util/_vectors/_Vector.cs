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

            AddSubContract(new("set", typeof(Vector3), typeof(Vector3),
                args: static exe =>
                {
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(int), out var expr_index))
                        exe.reader.Stderr($"expected {typeof(int)} expression.");
                    else
                    {
                        ExpressionExecutor expr_value = null;
                        if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(float), out expr_value))
                            exe.reader.Stderr($"expected {typeof(float)} expression.");
                        else
                        {
                            exe.args.Add(expr_index);
                            exe.arg_0 = expr_value;
                        }
                    }
                },
                routine: static exe =>
                {
                    ExpressionExecutor output_exe = ((SubContractExecutor)exe).output_exe;
                    ExpressionExecutor expr_index = (ExpressionExecutor)exe.args[0];

                    using var rout_output = output_exe.EExecute();
                    using var rout_index = expr_index.EExecute();
                    using var rout_value = exe.arg_0.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            int index = (int)rout_index.Current.output;
                            float value = (float)rout_value.Current.output;
                            Vector3 output = (Vector3)rout_output.Current.output;
                            output[index] = value;
                            return output;
                        },
                        rout_output,
                        rout_index,
                        rout_value);
                }));

            AddContract(new("vector3", typeof(Vector3),
                args: static exe =>
                {
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(float), out var expr_x))
                        exe.reader.Stderr($"expected x.");
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(float), out var expr_y))
                        exe.reader.Stderr($"expected y.");
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(float), out var expr_z))
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