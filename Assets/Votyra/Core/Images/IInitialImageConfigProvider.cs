using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;

namespace Votyra.Core.Images
{
	public interface IInitialImageConfigProvider
	{
		IInitialImageConfig GetInitialImageConfig();
	}
}