using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Escenario
{
    public abstract class ObjetoEscena
    {
        public TgcMesh mesh;
        public bool status = true;

        private string meshPath;

        protected GameModel env;
        private TgcSceneLoader loader;

        abstract protected string getMeshPath();

        public ObjetoEscena(GameModel env)
        {
            this.env = env;
            loader = new TgcSceneLoader();

            meshPath = getMeshPath();
            mesh = loader.loadSceneFromFile(meshPath).Meshes[0];
        }
        
        public void Update(float ElapsedTime)
        {
            
        }

        public void Render()
        {
            if(this.status)
            {
                this.mesh.render();
            }
        }

        public void Dispose()
        {
            this.mesh.dispose();
        }

        abstract public List<Objeto> Destroy();

        public void RotateY(float angle)
        {
            this.mesh.rotateY(angle);
        }

        private string RndString(int length)
        {
            Random random = new Random();
            StringBuilder buffer = new StringBuilder(length);
           
            while (length > 0)
            {
                var randomNumber = random.Next(97, 122);
                buffer.Append((char)randomNumber, 1);
                --length;
            }
            return buffer.ToString();
        }
    }
}
