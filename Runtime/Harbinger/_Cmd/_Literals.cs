namespace _BOA_
{
    partial class Harbinger
    {
        public static readonly Contract literal = new("literal", typeof(object),
            action: static cont =>
            {
                cont.stdout(cont.args[0]);
            });
    }
}