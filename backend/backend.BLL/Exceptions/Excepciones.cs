namespace backend.BLL.Exceptions;

/// <summary>El recurso pedido no existe (o no pertenece al usuario). La API responde 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string mensaje) : base(mensaje) { }
}

/// <summary>Se incumple una regla de negocio o una validación. La API responde 400.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string mensaje) : base(mensaje) { }
}

/// <summary>Credenciales inválidas. La API responde 401.</summary>
public class UnauthorizedBusinessException : Exception
{
    public UnauthorizedBusinessException(string mensaje) : base(mensaje) { }
}
