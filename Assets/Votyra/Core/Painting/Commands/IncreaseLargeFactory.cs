﻿using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class IncreaseLargeFactory : BaseFactory<IncreaseLarge>
    {
        public IncreaseLargeFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.IncreaseLarge;
    }
}
