Module FileLoadBuilder
    Public Function BuildFileLoad(fileLoadName As String) As Dictionary(Of Integer, List(Of NxAttribute))
        Dim exampleData As Dictionary(Of Integer, List(Of NxAttribute)) = New Dictionary(Of Integer, List(Of NxAttribute))
        Dim list1 As List(Of NxAttribute) = New List(Of NxAttribute)
        list1.Add(New NxString("Matter"))
        exampleData.Add(1, list1)
        Dim list2 As List(Of NxAttribute) = New List(Of NxAttribute)
        list2.Add(New NxString("Payee"))
        exampleData.Add(2, list2)
        Dim list3 As List(Of NxAttribute) = New List(Of NxAttribute)
        list2.Add(New NxString("Tax"))
        exampleData.Add(3, list2)
        Return exampleData ' Filled with Data Load Information, converted into dictionary
    End Function

    Public Function BuildFileLoadIdentity(fileLoadName As String) As Dictionary(Of String, Dictionary(Of Integer, List(Of NxAttribute)))
        Dim exampleData As Dictionary(Of String, Dictionary(Of Integer, List(Of NxAttribute)))

        Dim exampleData1 As Dictionary(Of Integer, List(Of NxAttribute)) = New Dictionary(Of Integer, List(Of NxAttribute))
        Dim list1 As List(Of NxAttribute) = New List(Of NxAttribute)
        list1.Add(New NxString("Matter"))
        exampleData1.Add(1, list1)
        Dim list2 As List(Of NxAttribute) = New List(Of NxAttribute)
        list2.Add(New NxString("Payee"))
        exampleData1.Add(2, list2)
        Dim list3 As List(Of NxAttribute) = New List(Of NxAttribute)
        list2.Add(New NxString("Tax"))
        exampleData1.Add(3, list2)
        exampleData.Add("Voucher", exampleData1)

        Dim exampleData2 As Dictionary(Of Integer, List(Of NxAttribute)) = New Dictionary(Of Integer, List(Of NxAttribute))
        Dim list4 As List(Of NxAttribute) = New List(Of NxAttribute)
        list4.Add(New NxString("Matter"))
        exampleData2.Add(1, list4)
        Dim list5 As List(Of NxAttribute) = New List(Of NxAttribute)
        list5.Add(New NxString("Payee"))
        exampleData2.Add(2, list5)
        Dim list6 As List(Of NxAttribute) = New List(Of NxAttribute)
        list6.Add(New NxString("Tax"))
        exampleData2.Add(3, list6)
        exampleData.Add("VchrCost", exampleData2)

        Dim exampleData3 As Dictionary(Of Integer, List(Of NxAttribute)) = New Dictionary(Of Integer, List(Of NxAttribute))
        Dim list7 As List(Of NxAttribute) = New List(Of NxAttribute)
        list7.Add(New NxString("Matter"))
        exampleData3.Add(1, list7)
        Dim list8 As List(Of NxAttribute) = New List(Of NxAttribute)
        list8.Add(New NxString("Payee"))
        exampleData3.Add(2, list8)
        Dim list9 As List(Of NxAttribute) = New List(Of NxAttribute)
        list9.Add(New NxString("Tax"))
        exampleData3.Add(3, list9)
        exampleData.Add("VchrTax", exampleData3)

        Return exampleData
    End Function
End Module
