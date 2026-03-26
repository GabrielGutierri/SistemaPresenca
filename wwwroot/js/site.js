/*Toast Javascript*/
$(document).ready(function () {

    var message = $('#AlertMessage').text();
    if (message !== '') {
        $('.toast').toast('show')
    }
});

/*Mascara input*/
$(document).ready(function () {

    $('#Phone').mask('(00) 00000-0000', { placeholder: "(xx) xxxxx-xxxx" });
    $('#Cpf').mask('000.000.000-00', { reverse: true, placeholder: "xxx.xxx.xxx-xx" });
    $('#CPF').mask('000.000.000-00', { reverse: true, placeholder: "xxx.xxx.xxx-xx" });
});

/*JS IMAGEM*/
function displayImage() {
    var oFReader = new FileReader();
    oFReader.readAsDataURL(document.getElementById("Image").files[0]);
    oFReader.onload = function (oFREvent) {
        document.getElementById("imgPreview").src = oFREvent.target.result;
    };
}

/*Pagination table*/
$(document).ready(function () {
    $('#table-attendance').DataTable();
});

/*AJAX Aula*/
function applyClassFilter() {
    var vIdMateria = document.getElementById('subject-dropdown').value;
    var vDataInicial = document.getElementById('data-inicial').value;
    var vDataFinal = document.getElementById('data-final').value;
    var vSituacao = document.getElementById('situacao-dropdown').value;

    $.ajax({
        url: "/ClassQuery/AdvancedClassQuery",
        data: { startDate: vDataInicial, endDate: vDataFinal, subjectId: vIdMateria, classStatus: vSituacao },
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoConsulta').innerHTML = dados;
            }
        },
    });
}

/*AJAX Listagem*/
function applyListingFilter() {
    var vCodigo = document.getElementById('codigo').value;
    var vtipoTabela = document.getElementById('tipoTabela').value;
    $.ajax({
        url: "/ListingQuery/GetAdvancedQueryData",
        data: { codigo: vCodigo, tipoTabela: vtipoTabela },
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoConsulta').innerHTML = dados;
            }
        },
    });
}


/*AJAX Presencas*/
function applyAttendanceFilter() {
    var vIdAluno = document.getElementById('student-dropdown').value;
    var vIdAula = document.getElementById('class-dropdown').value;
    $.ajax({
        url: "/AttendanceQuery/AdvancedAttendanceQuery",
        data: { studentId: vIdAluno, classId: vIdAula },
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoConsulta').innerHTML = dados;
            }
        },
    });
}

/*Abrir Modal Imagem*/
$(document).on("click", ".openImageDialog", function () {
    console.log('Ok');
    var myImageId = $(this).data('id');
    $(".modal-body #myImage").attr("src", myImageId);
});

/*Deletar Dados*/
function deleteStudent(id) {
    Swal.fire({
        title: 'Confirm record deletion?',
        text: "You won't be able to revert this action!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = '/Student/Delete?id=' + id;
        }
    })
}

function deleteTeacher(id) {
    Swal.fire({
        title: 'Confirm record deletion?',
        text: "You won't be able to revert this action!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = '/Teacher/Delete?id=' + id;
        }
    })
}

function deleteSubject(id) {
    Swal.fire({
        title: 'Confirm record deletion?',
        text: "You won't be able to revert this action!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = '/Subject/Delete?id=' + id;
        }
    })
}

function deleteClass(id) {
    Swal.fire({
        title: 'Confirm record deletion?',
        text: "You won't be able to revert this action!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = '/Class/Delete?id=' + id;
        }
    })
}
