$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url : '/Admin/Product/getall' },
        "columns" :  [
            { data: 'title', "width":"25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'autherName', "width": "20%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'productID',
                "render": function (data) {
                    return `<div class="w-75 btn-group" rol="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                            <a href="" class="btn btn-primary mx-2"> <i class="bi bi-trash-fill"></i> Delete </a>
                    </div >`
                },
                "width":"20%"
            }
        ]
    });
}

