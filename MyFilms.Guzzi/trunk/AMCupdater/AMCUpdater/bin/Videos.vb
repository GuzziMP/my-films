Public MustInherit Class Videos

    Implements IStoreData

    'Public ListVid�os As New List(Of Vid�o)
    Public Shared ListVid�os As New DataTable
    Public MustOverride Sub LoadData() Implements IStoreData.LoadData
    Public MustOverride Sub SaveData() Implements IStoreData.SaveData

End Class