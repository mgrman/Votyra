using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Raycasting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class UnityInputManager : MonoBehaviour
    {
        private IPaintCommand activeCommand;
        private string lastActiveCommand;

        [InjectOptional]
        private ILayerConfig layerConfig;

        [Inject]
        private IPaintingModel paintingModel;

        private bool processing;

        [Inject]
        private IRaycaster raycaster;

        [Inject(Id = "root")]
        private GameObject root;

        [Inject]
        private ITerrainConfig terrainConfig;

        protected void Update()
        {
            if (this.processing)
            {
                return;
            }

            this.processing = true;
            var cmdName = this.GetActiveCommand();
            var activeCommand = this.activeCommand;
            if (cmdName != this.lastActiveCommand)
            {
                activeCommand?.Dispose();
                activeCommand = this.InstantiateCommand(cmdName);
                this.activeCommand = activeCommand;
                this.lastActiveCommand = cmdName;
            }

            if (activeCommand == null)
            {
                this.processing = false;
                return;
            }

            var ray = this.GetRayFromPointer(Input.mousePosition, Camera.main);
            if (this.terrainConfig.AsyncInput)
            {
                Task.Run(() =>
                {
                    try
                    {
                        this.ProcessInputs(ray, activeCommand);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    finally
                    {
                        this.processing = false;
                    }
                });
            }
            else
            {
                this.ProcessInputs(ray, activeCommand);
                this.processing = false;
            }
        }

        private IPaintCommand InstantiateCommand(string cmdName)
        {
            if (cmdName == null)
            {
                return null;
            }

            var factory = this.paintingModel.PaintCommandFactories.FirstOrDefault(o => o.Action == cmdName);
            if (factory == null)
            {
                return null;
            }

            return factory.Create();
        }

        private string GetActiveCommand()
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                return null;
            }

            if (this.layerConfig != null)
            {
                if (!Input.GetKey((KeyCode)this.layerConfig.Layer))
                {
                    return null;
                }
            }

            var isFlat = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var isHole = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var isInverse = Input.GetMouseButton(1);

            var isLarge = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            string cmdName;
            if (isFlat)
            {
                if (isLarge)
                {
                    cmdName = KnownCommands.FlattenLarge;
                }
                else
                {
                    cmdName = KnownCommands.Flatten;
                }
            }
            else if (isHole)
            {
                if (isInverse)
                {
                    if (isLarge)
                    {
                        cmdName = KnownCommands.RemoveHoleLarge;
                    }
                    else
                    {
                        cmdName = KnownCommands.RemoveHole;
                    }
                }
                else
                {
                    if (isLarge)
                    {
                        cmdName = KnownCommands.MakeHoleLarge;
                    }
                    else
                    {
                        cmdName = KnownCommands.MakeHole;
                    }
                }
            }
            else
            {
                if (isInverse)
                {
                    if (isLarge)
                    {
                        cmdName = KnownCommands.DecreaseLarge;
                    }
                    else
                    {
                        cmdName = KnownCommands.Decrease;
                    }
                }
                else
                {
                    if (isLarge)
                    {
                        cmdName = KnownCommands.IncreaseLarge;
                    }
                    else
                    {
                        cmdName = KnownCommands.Increase;
                    }
                }
            }

            return cmdName;
        }

        private void ProcessInputs(Ray3f ray, IPaintCommand cmd)
        {
            var rayHit = this.raycaster.Raycast(ray);

            if (rayHit.AnyNan())
            {
                return;
            }

            var cell = rayHit.XY()
                .RoundToVector2i();

            cmd.UpdateInvocationValues(cell);
        }

        private Ray3f GetRayFromPointer(Vector3 screenPosition, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screenPosition);

            var cameraPosition = this.root.transform.InverseTransformPoint(ray.origin)
                .ToVector3F();

            var cameraDirection = this.root.transform.InverseTransformDirection(ray.direction)
                .ToVector3F();

            var cameraRay = new Ray3f(cameraPosition, cameraDirection);

            return cameraRay;
        }
    }
}
