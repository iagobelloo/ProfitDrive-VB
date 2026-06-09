Imports System

Public Class Despesa
    Public Property Id As Integer
    Public Property UsuarioId As Integer
    Public Property Data As DateTime
    Public Property Valor As Decimal
    Public Property Descricao As String
    Public Property Categoria As String
    Public Property LancamentoId As Nullable(Of Integer)
End Class