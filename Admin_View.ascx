<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Admin_View.ascx.cs" Inherits="CNTRCovidForm.Views.Admin_View" %>
<%@ Import namespace="GCUserLookupService.Security" %>
<link rel="stylesheet" type="text/css" href="/ics/ClientConfig/css/bootstrap.css" />
<script src="/ics/Portlets/CUS/ICS/GCUserLookupService/GCUserLookup.js" type="text/javascript"></script>

<script src="/ics/ClientConfig/DataTables/dataTables.min.js" type="text/javascript"></script>
<link href="/ics/ClientConfig/DataTables/dataTables.min.css" rel="stylesheet" type="text/css" />


<asp:HiddenField ID="hf_isButtonClicked" runat="server" OnValueChanged="changeView" Value="default"/>
<asp:HiddenField ID="hf_row" runat="server" Value="default"/>
<asp:HiddenField ID="hf_Range" runat="server" Value="default"/>
<asp:HiddenField ID ="hf_Submission" runat="server" Value="default" />
<asp:HiddenField ID="hf_Current_Index" runat="server" Value="default"/>
<asp:HiddenField ID="hf_OrderBy" runat="server" Value="default"/>
<asp:HiddenField ID="hf_current_Viewable" runat="server" Value="default"/>
<div>
    <h2><asp:Label ID="searchLabel" runat="server" >Submission List View</asp:Label></h2>
</div>

<hr />
<h3><asp:Label ID="currentPortlet" runat="server">Study Abroad Submissions</asp:Label></h3>
<div class="pSection">
    <div class="pSection">
        <asp:Label ID="listableLabel" runat="server">Current Form:</asp:Label>
        <asp:DropDownList ID="listableList" AutoPostBack="true" OnSelectedIndexChanged="ddl_ListableList_clicked" runat="server"></asp:DropDownList>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="editModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLongTitle" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="exampleModalLongTitle">Edit Objectable</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
            
          <div ID="div_modal_body" class="modal-body" runat="server">
            <asp:Panel ID="pnlEditModal" ClientIDMode="static" runat="server"></asp:Panel>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
          </div>
        </div>
      </div>
    </div>

    <div class ="col-lg-12 panel panel-primary">
        <asp:Label id="div_Listable_header_Title" class="panel-heading" runat="server">Study Abroad Submission Information</asp:Label>
        <asp:Panel id="div_Listable_header" class="panel-body" runat="server"></asp:Panel>
    </div>
    <asp:Panel id="div_Listable_body" class ="col-lg-12" runat="server">
        <asp:HiddenField ID="hf_Selected_Object" runat="server" />
        <div class="table-responsive-lg ">
        <asp:table id='viewable_Table' class='table table-striped table-bordered dataTable tfoot td' style='width:100%' runat="server"></asp:table>
        </div>
    </asp:Panel>
    <asp:Panel id="div_Listable_footer" class ="col-lg-12" runat="server"></asp:Panel>
</div>

<script>
    
    $(document).ready(function () {
        var columns;
        var column_Length;
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/GetColumns",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log('Success');
                console.log(data.d);
                columns = data.d;
                column_Length = columns.length
                
                var json_Columns = [];
                var i;
                for (i = 0; i < column_Length; i++){
                json_Columns.push({ "data": columns[i], "orderable": true, "title": columns[i], "class":columns[i], "defaultContent": "<input type='button' value='Print' class='btn btn-dark' />" });
                }
                console.log(json_Columns)
                 $('.dataTable').prepend($("<thead></thead>").append($(this).find("tr:first"))).dataTable({
                    "processing": true,
                    "serverSide": true,
                    "info": true,
                    "stateSave": true,
                    "lengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
                    "ajax": {
                        url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/GetData",
                        type: "POST"
                        //dataSrc: ""
                        //dataType: "json",
                        //contentType: "application/json; charset=utf-8"
                    },
                    "columns": json_Columns,
                    "order":[[0,"asc"]]
                });

                $("#tabs-list li").click(function () {
                    $('.tab-box').hide().eq($(this).index()).show()
                    $("#tabs-list li").removeClass('active');
                    $("#tabs-list li").eq($(this).index()).addClass('active');  // hide all divs and show the current div
                });

                $('.dataTable tbody').on('click', 'input', function () {
                    var action = this.value;
					var tr = $(this).closest('tr');
					var a = tr.find('td:eq(1) input').val();
                    var row_indx = $('.dataTable').DataTable().row($(this).parents('tr')).index();
					var submissionRow =  $('.dataTable').DataTable().row(row_indx).cells().length;
					var column_length = $('.dataTable').DataTable().columns().header().length;
					var submission_ID =  $('.dataTable').DataTable().cell({row: row_indx, column:column_length-2}).data();
                    console.log(row_indx);
					console.log("A ="+a+" Submission Row:"+ submissionRow+"<br>Submission ID:"+submission_ID+" Raw Data:"+$('.dataTable').DataTable().rows().data());
                    $('#<%=hf_row.ClientID%>').val(row_indx);
					$('#<%=hf_Submission.ClientID%>').val(submission_ID);
                    if (action == 'Edit') {
                        edit_clicked(row_indx);
                    }
                    else if (action == 'Review') {
                        review_clicked(row_indx);
                    }
                    else if (action == 'Delete') {
                        delete_clicked(row_indx);
                    }
                    else if (action == 'Print') {
                        print_clicked(row_indx);
                    }
                })
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); }
        })
        
    });
    function print_clicked(row) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/print_Clicked",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log(data.d);
                $('#<%=hf_isButtonClicked.ClientID%>').val(data.d);
                __doPostBack();
                
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); console.log(err)}
        })

    }
    function edit_clicked(row) {
        
        var dataString = "{ row: " + JSON.stringify(row) + "}";
        console.log(dataString);

        $.ajax({
            type: "POST",
            dataType: 'json',
            data: dataString,
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/edit_Clicked",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log(data.d);
                $('#<%=hf_isButtonClicked.ClientID%>').val(data.d);
                __doPostBack();
                
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); console.log(err)}
        })
    }
    function review_clicked(row) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/review_Clicked",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log(data.d);
                $('#<%=hf_isButtonClicked.ClientID%>').val(data.d);
                __doPostBack();
                
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); console.log(err)}
        })
    }
    function delete_clicked(row) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/delete_Clicked",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log(data.d);
                $('#<%=hf_isButtonClicked.ClientID%>').val(data.d);
                __doPostBack();
                
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); console.log(err)}
        })
    }

  function UpdateTable() {
        var table = $(".dataTable").DataTable({
            "iDisplayLength": 25,
            "aoColumnDefs": [{ 'bSortable': false, 'aTargets': [5] }],
            "aaSorting": [[0, "asc"], [1, "asc"]],
            "bStateSave": true,
            responsive: true
        });
        table.column(6).visible(false);

        //setup input text for footer
        $('.dataTable tfoot td').each(function () {
            var title = $(this).text();
            if (title != "Edit" && title != "Div") {
                $(this).html('<input type="text" placeholder="' + title + '" />');
            }
        });

        // Apply the search
        table.columns().every(function () {
            var that = this;

            $('input', this.footer()).on('keyup change', function () {
                if (that.search() !== this.value) {
                    that
                        .search(this.value)
                        .draw();
                }
            });
        });
    }

    function getColumnList() {
        var colunm_List;
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ics/Portlets/CUS/ICS/CNTRCovidForm/lib/CovidFormWebService.asmx/GetColumns",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                console.log('Success');
                console.log(data.d);
                colunm_List = data.d;
                return colunm_List;
            },
            error: function (jqX, status, err) { console.log('Ajax failure'); }
        })
        
        
    }
</script>
