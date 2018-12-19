using System;

namespace PipelineActors.Support {
   public class CorrelationId {
      public Guid Value { get; }

      public CorrelationId() {
         Value = Guid.NewGuid();
      }

      public CorrelationId( Guid value ) {
         Value = value;
      }

      public static implicit operator Guid( CorrelationId id ) {
         return id.Value;
      }

      public static implicit operator CorrelationId( Guid value ) {
         return new CorrelationId( value );
      }
   }
}