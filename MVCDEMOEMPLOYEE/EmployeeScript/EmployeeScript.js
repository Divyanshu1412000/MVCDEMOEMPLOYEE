
// Show Employee Data
$(document).ready(function () {
    getDesignationDropdown();
    getDepartmentCheck();
    getEmployees();
});

$("#inactiveEmpModal").appendTo("body")
$("#employeeModal").appendTo("body")

var uploadedFileName = ""; // To store the uploaded file name


$("#submit").click(function () {

    // Trigger file upload before submitting other data
    var fileUploadSuccess = uploadFile();

    if (!fileUploadSuccess) {
        alert("File upload failed.");
        return false;
    }

    var department = '';
    //for (var i = 0; i < $('#departmentCheckboxes').find('input').length; i++) {
    //    if ($("#departmentCheckboxes").find('input')[i].checked) {
    //        department += (department ? ',' : '') + $('#departmentCheckboxes').find('input')[i].value;
    //    }
    //}
    var ddlDepartmentss = $("#ddlDepartmentss").val();
    
    for (var i = 0; i < ddlDepartmentss.length; i++) {
        department += (department ? ',' : '') + ddlDepartmentss[i]
    }
    //console.log(department);
    //return false;

    if ($('#empCode').val() == '') {
        iziToast.warning({
            message: 'Please Enter Employee Code'
        });
        return false;
    }

    if ($('#empName').val() == '') {
        iziToast.warning({
            message: 'Please Enter Employee Name'
        });
        return false;
    }

    //validation for multiple checkboxes
    //if (!$('.deptCheckbox').is(':checked')) {
    //    iziToast.warning({
    //        message: 'Please select alteast one Department'
    //    });
    //    return false;
    //}

    //if (!$("#ddlDepartmentss").is(':selected')) {
    //    iziToast.warning({
    //        message: 'Please select alteast one Department'
    //    });
    //    return false;
    //}

    if ($("#ddlDepartmentss").val() =='') {
        iziToast.warning({
            message: 'Please select alteast one Department'
        });
        return false;
    }

    if ($("#ddlDesignation").val() === "0") {
        iziToast.warning({
            message: 'Please select Designation '
        });
        return false;
    }

    if (!$("input[name='gender']:checked").val()) {
        iziToast.warning({
            message: 'Please select Gender'
        });
        return false;
    }


    var empData = {
        empId: $('#empId').val(),
        empCode: $('#empCode').val(),
        empName: $('#empName').val(),
        gender: $('input[name="gender"]:checked').val(),
        SelectedDesignation: $('#ddlDesignation').val(),
        Dept: department,
        ImagePath: uploadedFileName
    }


    $.ajax({
        url: "/Employee/Insert",
        type: "POST",
        data: empData,
        beforeSend: function () {
            console.log(empData)
        },
        success: function (response) {
            if (response.success) {
                if (empData.empId == 0) {
                    alert("Data Successfully Inserted");
                }
                else {
                    alert("Data Successfully Updated");

                }
                getEmployees();
                resetForm();
                $("#submit").html("Submit");
                $("#empId").prop('value', 0);

            }
            else {
                alert("Failed to insert Employee");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }
    });


});


// Separate function for file upload
function uploadFile() {
    var file = $("#profilePic")[0].files[0];  // Get the file from input
    if (file) {
        var formData = new FormData();
        formData.append("file", file);

        var fileUploadSuccess = false;

        $.ajax({
            url: "/Employee/UploadFile",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                if (response.success) {
                    uploadedFileName = response.filePath; // Store returned file path
                    fileUploadSuccess = true;
                } else {
                    fileUploadSuccess = false;
                }
            },
            error: function () {
                alert("Error uploading file.");
                fileUploadSuccess = false;
            }
        });

        return fileUploadSuccess;
    }

    return true;
}

function getDesignationDropdown() {
    $.ajax({
        url: "/Employee/GetDesignations",
        type: "GET",
        async: false,
        success: function (data) {
            //console.log(data);
            $.each(data, function (index, option) {
                //$('#ddlDesignation').append($('<option></option>').attr('value', option.id).text(option.designationName));
                // $('#ddlDesignation').append('<option value="'++'">' + option.designationName +'</option>');
                $('#ddlDesignation').append(`<option value="${option.id}">${option.designationName}</option>`);
            });
        }
    });
    $("#ddlDesignation").select2();
}

function getDepartmentCheck() {
    $.ajax({
        url: "/Employee/GetDepartments",
        type: "GET",
        success: function (data) {
            //console.log(data);
            $.each(data, function (index, option) {
                $('#ddlDepartmentss').append(`<option  value="${option.id}">${option.deptName}</option>`);
            });
        
            $('#ddlDepartmentss').multiselect();
        }
    });
}

function getEmployees() {
    $.ajax({
        url: "/Employee/getEmployees",
        type: "GET",
        success: function (data) {
            $("#tblEmployee").DataTable().destroy()
            $("#employeeTableBody").empty();
            $("#employeeInActiveTableBody").empty();



            console.log(data)
            var isActiveIndex = 0
            var isInActiveIndex = 0
            $.each(data, function (index, emp) {

                var genderVal = emp.gender === 0 ? "Male" : "Female";

                if (emp.isActiveEmp == 1) {

                    var row = "<tr>" +
                        "<td>" + (isActiveIndex += 1) + "</td>" +
                        "<td>" + emp.empCode + "</td>" +
                        "<td>" + emp.empName + "</td>" +
                        "<td>" + genderVal + "</td>" +
                        "<td>" + emp.Dept + "</td>" +
                        "<td>" + emp.Designation + "</td>" +
                        "<td><div class='d-flex'><button class=' me-1 btn btn-primary btn-sm' onclick='UpdateEmp(" + emp.empId + ")'><i class='bi bi-pencil'></i></button>" +
                        "<button class='me-1 btn btn-danger  btn-sm' onclick = 'deleteEmp(" + emp.empId + ")' ><i class='bi bi-trash'></i></button >" +
                        "<button class='btn btn-success  btn-sm' onclick='ViewEmp(" + emp.empId + ")'><i class='bi bi-eye'></i></button></div ></td > " +

                        "</tr>";
                    $("#employeeTableBody").append(row);
                }
                else {

                    var row = "<tr>" +
                        "<td>" + (isInActiveIndex += 1) + "</td>" +
                        "<td>" + emp.empCode + "</td>" +
                        "<td>" + emp.empName + "</td>" +
                        "<td>" + genderVal + "</td>" +
                        "<td>" + emp.Dept + "</td>" +
                        "<td>" + emp.Designation + "</td>" +
                        "<td><div class='d-flex'><button class='btn btn-primary btn-sm me-1' onclick='UpdateEmp(" + emp.empId + ")'><i class='bi bi-pencil'></i></button>" +
                        "<button class='btn btn-info  me-1  btn-sm' onclick='makeActive(" + emp.empId + ")'><i class='bi bi-check'></i></button>" +
                        "<button class='btn btn-success  btn-sm' onclick='ViewEmp(" + emp.empId + ")'><i class='bi bi-eye'></i></button></div></td>"

                    "</tr>";
                    $("#employeeInActiveTableBody").append(row);
                }

            });

            $("#tblEmployee").DataTable();

            $("#employeeGrid").show();
        }
    });
};
function deleteEmp(id) {
    var empId = id;
    console.log(id);
    $("#empId").val(empId);
    resetForm()
    $.ajax({
        url: "/Employee/delEmpId",
        type: "POST",
        data: { id: empId },
        success: function (emp) {
            getEmployees();
            //change button text
            console.log(emp)
        }
    });

};


function makeActive(id) {
    var empId = id;
    console.log(id);
    $("#empId").val(empId);
    resetForm()
    $.ajax({
        url: "/Employee/MakeActive",
        type: "POST",
        data: { id: empId },
        success: function (emp) {
            getEmployees();
            //change button text
            console.log(emp)
        }
    });

}

function UpdateEmp(id) {
    var empId = id;
    console.log(id);
    $("#empId").val(empId);
    resetForm()
    $.ajax({
        url: "/Employee/getEmpId",
        type: "POST",
        data: { id: empId },
        success: function (emp) {
            //change button text
            $("#submit").html("Update")
            $('#empCode').val(emp.empCode),
                $('#empName').val(emp.empName),
                $("#ddlDesignation").val(emp.designationId).select2(),
                $("#profilePic").attr('src', emp.ImagePath)


            var DeptValue = emp.Dept;

            var DeptArray = DeptValue.split(",");

            $('#ddlDepartmentss').val(DeptArray);
            $('#ddlDepartmentss').multiselect('refresh');

            if (emp.gender === 0) {
                $("#male").prop('checked', true);
            }
            else {
                $("#female").prop('checked', true);
            }
        }
    });
}


$("#reset").click(function () {
    resetForm();
});

function resetForm() {
    $("#empCode").val('')
    $("#empName").val('')

    $('#ddlDepartmentss option:selected').prop('selected', false);
   /* $('#ddlDepartmentss option').remove();*/
    $('#ddlDepartmentss').multiselect('rebuild');

    $('input[name="gender"]').prop('checked', false)

    $("#ddlDesignation").val(0).select2()

    $("#profilePic").val('')

    $("#submit").text("Submit")
}

function ViewEmp(id) {

    console.log("empId: " + id);
    $.ajax({
        url: "/Employee/ViewEmployee",
        type: "POST",
        data: { id: id },

        success: function (emp) {
            var genderVal = emp.gender === 0 ? "Male" : "Female";
            $("#modalempCode").text(emp.empCode),
                $("#modalempName").text(emp.empName),
                $("#modalempGender").text(genderVal),
                $("#modalempDept").text(emp.Dept),
                $("#modalempDesignation").text(emp.Designation),
                $("#modalempImage").attr("src", emp.ImagePath);
            $("#employeeModal").modal("show");
            if (emp.ImagePath != "") {

                $(".displayImage").show()
            }
            else {
                $(".displayImage").hide()
            }
            // Set multi-select dropdown values
            var deptArray = emp.Dept.split(",");
            $('#ddlDepartment').val(deptArray).trigger("change");
            console.log(emp);
        }
        
    });

}

function downloadButton() {
    var imageUrl = $("#modalempImage").attr("src");
    var link = document.createElement("a");
    link.href = imageUrl;
    link.download = imageUrl;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}