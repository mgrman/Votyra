using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Unity;
using Zenject;

namespace Votyra.Core
{
    public static class TerrainDataControllerConfigUtilities
    {
        public static IEnumerable<Type> GetConfigTypes(GameObject algorithPrefab)
        {
            IEnumerable<IInstaller> installers;

            var autoContext = algorithPrefab.GetComponent<AutoGameObjectContext>();
            if (autoContext != null)
            {
                installers = autoContext.GetInstallers();
            }
            else
            {
                var context = algorithPrefab.GetComponent<AutoGameObjectContext>();
                installers = context.Installers;
            }

            var container = new DiContainer(false);
            var tempGameObject = new GameObject();
            try
            {
                foreach (var installer in installers)
                {
                    var tempInstallerGo = tempGameObject.AddComponent(installer.GetType());
                    var tempInstaller = tempInstallerGo.GetComponentInChildren<MonoInstallerBase>(true);

                    try
                    {
                        container.Inject(tempInstaller, new object[0]);
                        tempInstaller.InstallBindings();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        UnityEngine.Object.DestroyImmediate(tempInstaller);
                    }
                }

                container.FlushBindings();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tempGameObject);
            }

            var types = new List<Type>();
            foreach (var provider in container.AllProviders)
            {
                try
                {
                    var instanceType = provider.GetInstanceType(new InjectContext(container, typeof(object)));
                    types.Add(instanceType);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            return types;
        }
    }
}