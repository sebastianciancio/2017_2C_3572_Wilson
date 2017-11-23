using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Utils;
using TGC.Core.Collision;

namespace TGC.Group.Model.Camara
{
    /// <summary>
    ///     Camara en primera persona que utiliza matrices de rotacion, solo almacena las rotaciones en updown y costados.
    ///     Ref: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Mouse_camera.php
    ///     Autor: Rodrigo Garcia.
    /// </summary>
    public class TgcFpsCamera : TgcCamera
    {
        private readonly Point mouseCenter; //Centro de mause 2D para ocultarlo.

        //Se mantiene la matriz rotacion para no hacer este calculo cada vez.
        private Matrix cameraRotation;

        //Direction view se calcula a partir de donde se quiere ver con la camara inicialmente. por defecto se ve en -Z.
        private Vector3 directionView;        

        //No hace falta la base ya que siempre es la misma, la base se arma segun las rotaciones de esto costados y updown.
        private float leftrightRot;
        private float updownRot;

        private bool lockCam;
        private Vector3 positionEye { get; set; } = new Vector3();
        private GameModel env;

        public TgcFpsCamera(TgcD3dInput input)
        {
            Input = input;
            positionEye = new Vector3();
            mouseCenter = new Point(
                D3DDevice.Instance.Device.Viewport.Width / 2,
                D3DDevice.Instance.Device.Viewport.Height / 2);
            RotationSpeed = 0.01f;
            MovementSpeed = 50f;
            JumpSpeed =50f;
            directionView = new Vector3(0, 0, -1);
            leftrightRot = FastMath.PI_HALF;
            updownRot = -FastMath.PI / 10.0f;
            cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
        }

        public TgcFpsCamera(Vector3 positionEye, TgcD3dInput input) : this(input)
        {
            this.positionEye = positionEye;
        }

        public TgcFpsCamera(GameModel env, Vector3 positionEye, float moveSpeed, float jumpSpeed, TgcD3dInput input)
            : this(positionEye, input)
        {
            MovementSpeed = moveSpeed;
            JumpSpeed = jumpSpeed;
            this.env = env;
        }

        /*public TgcFpsCamera(Vector3 positionEye, float moveSpeed, float jumpSpeed, float rotationSpeed,
            TgcD3dInput input) : this(positionEye, moveSpeed, jumpSpeed, input)
        {
            RotationSpeed = rotationSpeed;
        }*/

        private TgcD3dInput Input { get; }

        public bool LockCam
        {
            get { return lockCam; }
            set
            {
                if (!lockCam && value)
                {
                    Cursor.Position = mouseCenter;

                    Cursor.Hide();
                }
                if (lockCam && !value)
                    Cursor.Show();
                lockCam = value;
            }
        }

        public float MovementSpeed { get; set; }

        public float RotationSpeed { get; set; }

        public float JumpSpeed { get; set; }

        /// <summary>
        ///     Cuando se elimina esto hay que desbloquear la camera.
        /// </summary>
        ~TgcFpsCamera()
        {
            LockCam = false;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            var moveVector = new Vector3(0, 0, 0);

            // Al mover el mouse se mueve la cabeza (genera mayor realismo)
            LockCam = true;

            //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
            var lastPos = this.Position;

            if (!env.personaje.Muerto && !env.partidoGanado)
            {

                //Forward
                if (Input.keyDown(Key.W))
                {
                    // Fuerzo a que el Personaje este siempre sobre la Isla
                    if (env.terreno.estaDentroTerreno())
                    {
                        moveVector += new Vector3(0, 0, -1) * MovementSpeed;
                    }else{
                        moveVector -= new Vector3(0, 0, -10) * MovementSpeed;
                    }
                }

                //Backward
                if (Input.keyDown(Key.S))
                {
                    // Fuerzo a que el Personaje este siempre sobre la Isla
                    if (env.terreno.estaDentroTerreno())
                    {
                        moveVector += new Vector3(0, 0, 1) * MovementSpeed;
                    }
                    else{
                        moveVector -= new Vector3(0, 0, 10) * MovementSpeed;
                    }
                }

                //Strafe right
                if (Input.keyDown(Key.D))
                {
                    // Fuerzo a que el Personaje este siempre sobre la Isla
                    if (env.terreno.estaDentroTerreno())
                    {
                        moveVector += new Vector3(-1, 0, 0) * MovementSpeed;
                    }
                    else{
                        moveVector -= new Vector3(-10, 0, 0) * MovementSpeed;
                    }
                }

                //Strafe left
                if (Input.keyDown(Key.A))
                {
                    // Fuerzo a que el Personaje este siempre sobre la Isla
                    if (env.terreno.estaDentroTerreno())
                    {
                        moveVector += new Vector3(1, 0, 0) * MovementSpeed;
                    }
                    else{
                        moveVector -= new Vector3(10, 0, 0) * MovementSpeed;
                    }
                }

                // Gods
                if (Input.keyDown(Key.G))
                {
                    env.alturaCamara = 200f;
                    env.modoDios = true;
                }

                // Normal
                if (Input.keyDown(Key.N))
                {
                    env.alturaCamara = 10f;
                    env.modoDios = false;
                }

                // Teclas de Menu
                if (Input.keyPressed(Key.UpArrow) && env.opcionMenuSelecionado > 0)
                {
                    env.opcionMenuSelecionado -= 1;
                }
                if (Input.keyPressed(Key.DownArrow) && env.opcionMenuSelecionado < 2)
                {
                    env.opcionMenuSelecionado += 1;
                }


                switch (env.opcionMenuSelecionado)
                {
                    case 0:
                        env.buttonSelected.Position = new Vector2(((float)D3DDevice.Instance.Width / 2) - 150, 210);
                        break;
                    case 1:
                        env.buttonSelected.Position = new Vector2(((float)D3DDevice.Instance.Width / 2) - 150, 280);
                        break;
                    case 2:
                        env.buttonSelected.Position = new Vector2(((float)D3DDevice.Instance.Width / 2) - 150, 350);
                        break;
                }

                //Run
                if (Input.keyDown(Key.LeftShift))
                {
                    // Fuerzo a que el Personaje este siempre sobre la Isla
                    if (env.terreno.estaDentroTerreno())
                    {
                        moveVector += new Vector3(0, 1, 0) * JumpSpeed;
                    }
                    else{
                        moveVector -= new Vector3(0, 10, 0) * JumpSpeed;
                    }
                }
        }


            //Detectar colisiones
            var collide = false;
            foreach (var obstaculo in env.terreno.SceneMeshes)
            {

                if (TgcCollisionUtils.testSphereAABB(env.personaje.BoundingSphere, obstaculo.BoundingBox))
                {
                    env.personaje.BoundingSphere.setRenderColor(Color.Red);
                    collide = true;
                    break;
                }
            }

            if (!env.personaje.Muerto && !env.partidoGanado)
            {
                //Si hubo colision, restaurar la posicion anterior de la camara
                if (collide)
                {
                    moveVector -= new Vector3(0, 0, -1) * MovementSpeed;
                }

                // Aplico la rotacion de la camara
                leftrightRot -= -Input.XposRelative * RotationSpeed;
                updownRot -= Input.YposRelative * RotationSpeed;
                //Se actualiza matrix de rotacion, para no hacer este calculo cada vez y solo cuando en verdad es necesario.
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);

                if (lockCam)
                    Cursor.Position = mouseCenter;

                //Calculamos la nueva posicion del ojo segun la rotacion actual de la camara.
                var cameraRotatedPositionEye = Vector3.TransformNormal(moveVector * elapsedTime, cameraRotation);
                positionEye += cameraRotatedPositionEye;

                //Calculamos el target de la camara, segun su direccion inicial y las rotaciones en screen space x,y.
                var cameraRotatedTarget = Vector3.TransformNormal(directionView, cameraRotation);
                var cameraFinalTarget = positionEye + cameraRotatedTarget;

                var cameraOriginalUpVector = DEFAULT_UP_VECTOR;
                var cameraRotatedUpVector = Vector3.TransformNormal(cameraOriginalUpVector, cameraRotation);

                base.SetCamera(positionEye, cameraFinalTarget, cameraRotatedUpVector);

                // Posiciono la Camara a la Altura del Terreno según las coordenadas actuales
                var posicionCamaraTerrenoOriginal = env.terreno.CalcularAlturaTerreno(
                        FastMath.Ceiling(positionEye.X / env.terreno.SceneScaleXZ),
                        FastMath.Ceiling(positionEye.Z / env.terreno.SceneScaleXZ)
                    ,true) + env.alturaCamara;

                var newpositionEye = new Vector3(positionEye.X, posicionCamaraTerrenoOriginal * env.terreno.SceneScaleY, positionEye.Z);
                var newcameraFinalTarget = newpositionEye + cameraRotatedTarget;

                base.SetCamera(newpositionEye, newcameraFinalTarget, cameraRotatedUpVector);


                //var posicionHacha = new Vector3(env.personaje.hachaPersonaje.Position.X + leftrightRot - 1, env.personaje.hachaPersonaje.Position.Y, env.personaje.hachaPersonaje.Position.Z);
                

                //Vector3 dirView = env.Camara.LookAt - env.Camara.Position;

                //env.personaje.hachaPersonaje.Transform = Matrix.Translation(posicionHacha) * cameraRotation;

                //env.personaje.hachaPersonaje.Transform = env.Camara.LookAt;
                //env.personaje.hachaPersonaje.Rotation = cameraRotatedPositionEye; 
                //env.personaje.hachaPersonaje.Rotation = dirView;

            }



        }

        /// <summary>
        ///     se hace override para actualizar las posiones internas, estas seran utilizadas en el proximo update.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="directionView"> debe ser normalizado.</param>
        public override void SetCamera(Vector3 position, Vector3 directionView)
        {
            positionEye = position;
            this.directionView = directionView;
        }

    }
}