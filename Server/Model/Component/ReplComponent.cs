using System;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ETModel
{
    [ObjectSystem]
    public class ReplComponentAwakeSystem : AwakeSystem<ReplComponent>
    {
        public override void Awake(ReplComponent self)
        {
            self.ScriptOptions = ScriptOptions.Default
                    .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(Environment.CurrentDirectory))
                    .AddReferences(typeof (ReplComponent).Assembly)
                    .AddImports("System");
        }
    }
    
    public class ReplComponent: Component
    {
        public ScriptOptions ScriptOptions;
        public ScriptState ScriptState;

        public async ETTask Run(string line, CancellationToken cancellationToken)
        {
            if (this.ScriptState == null)
            {
                this.ScriptState = await CSharpScript.RunAsync(line, this.ScriptOptions, cancellationToken: cancellationToken);
            }
            else
            {
                this.ScriptState = await this.ScriptState.ContinueWithAsync(line, cancellationToken: cancellationToken);
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
            this.ScriptOptions = null;
            this.ScriptState = null;
        }
    }
}