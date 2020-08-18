<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Default_View.ascx.cs" Inherits="CNTRCovidForm.Views.Default_View" %>

<link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.6/themes/base/jquery-ui.css" type="text/css" media="all">
<style>
  #feedback { font-size: 1.4em; }
  #selectable .ui-selecting { background: #000000; color:#FFFFFF; }
  #selectable .ui-selected { background: #FFCC00; color: white; }
  #selectable { list-style-type: none; margin: 0; padding: 0; width: 100%; text-align:center; }
  #selectable li { font-size:x-large; border-radius:10px;}

  #centre-selectable .ui-selecting { background: #000000; color:#FFFFFF; }
  #centre-selectable .ui-selected { background: #FFCC00; color: white; }
  #centre-selectable { list-style-type: none; margin: 0; padding: 0; width: 100%; text-align:center; }
  #centre-selectable li { font-size:x-large; border-radius:10px;}

  .centre_DropDown {width:100%; font-size:x-large; background:#FFFFFF; color:#000000; border-radius:10px;}
  .centre_Label{font-size:large}
  .centre_TextBox{width:100%; font-size:large }
  .row{width:100%}
  .centre-row {margin: 0 0 .5em 0 !important;}
  .cntr-clearance{
    list-style: decimal !important;
    text-align: left !important;
    font-size: 20px !important;
  }
  .cntr-center{
      text-align: center !important;
  }
  .cntr-btn{      
    margin-bottom: 10px;
  }
  </style>
<script>
    $(document).ready(function () {
        $('#<%=hf_Saveable.ClientID%>').val("false");
        var element = document.getElementById("div_OtherSymptoms");        
        if (element != null)
            element.style.display = "none";
        $('.symptoms_Row').on('click', 'input', function () {
            var action = this.value;
            check_Saveable();
            var selected_Symptoms = document.getElementsByClassName('ui-selectee')
            if (selected_Symptoms != null) {
                var symptoms_Length = selected_Symptoms.length;
                for (var i = 0; i < symptoms_Length; i++) {
                    //console.log(selected_Symptoms[i]);
                    switch (selected_Symptoms[i].id) {
                        case "temp":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Temp.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Temp.ClientID%>').val("false"); }
                            break;
                        case "chills":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Chills.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Chills.ClientID%>').val("false"); }
                            break;
                        case "cough":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Cough.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Cough.ClientID%>').val("false"); }
                            break;
                        case "breath":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Breath.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Breath.ClientID%>').val("false"); }
                            break;
                        case "fatigue":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Fatigue.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Fatigue.ClientID%>').val("false"); }
                            break;                        
                        case "musclepain":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_MusclePain.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_MusclePain.ClientID%>').val("false"); }
                            break;
                         case "headache":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Headache.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Headache.ClientID%>').val("false"); }
                            break;
                        case "smellortaste":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_SmellOrTaste.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_SmellOrTaste.ClientID%>').val("false"); }
                            break;
                        case "throat":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Throat.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Throat.ClientID%>').val("false"); }
                            break;
                        case "congestion":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Congestion.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Congestion.ClientID%>').val("false"); }
                            break;
                        case "nausea":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Nausea.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Nausea.ClientID%>').val("false"); }
                            break;
                        case "diarrhea":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Diarrhea.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Diarrhea.ClientID%>').val("false"); }
                            break;
                        case "quarantined":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Quarantined.ClientID%>').val("true");
                            }
                            else { $('#<%=hf_Quarantined.ClientID%>').val("false"); }
                        case "noSymptoms":
                            if (selected_Symptoms[i].classList.contains("ui-selected")) {
                                $('#<%=hf_Temp.ClientID%>').val("false");
                                $('#<%=hf_Chills.ClientID%>').val("false");
                                $('#<%=hf_Cough.ClientID%>').val("false");
                                $('#<%=hf_Breath.ClientID%>').val("false");
                                $('#<%=hf_Fatigue.ClientID%>').val("false");
                                $('#<%=hf_MusclePain.ClientID%>').val("false");
                                $('#<%=hf_Headache.ClientID%>').val("false");
                                $('#<%=hf_SmellOrTaste.ClientID%>').val("false");
                                $('#<%=hf_Throat.ClientID%>').val("false");
                                $('#<%=hf_Congestion.ClientID%>').val("false");
                                $('#<%=hf_Nausea.ClientID%>').val("false");
                                $('#<%=hf_Diarrhea.ClientID%>').val("false");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        });
    });

    $(function () {
        $("#centre-selectable").selectable({
            selected: function (event, ui) {
                check_Saveable();
                var el = $(ui.selected);
                switch (el.attr('id')) {
                    case "onCampus":
                        $('#<%=hf_OnCampus.ClientID%>').val("true");
                        $("#offCampus").removeClass("ui-selected");
                        break;
                    case "offCampus":
                        $('#<%=hf_OnCampus.ClientID%>').val("false");
                        $("#onCampus").removeClass("ui-selected");
                        break;
                }
            },
            unselected: function (event, ui) {
                check_Saveable();
                var el = $(ui.selected);
                switch (el.attr('id')) {
                    case "onCampus":
                        $('#<%=hf_OnCampus.ClientID%>').val("false");
                        break;
                    case "offCampus":
                        $('#<%=hf_OnCampus.ClientID%>').val("true");
                        break;
                }
            }
        });
    });

    $(function() {
        $("#selectable").bind("mousedown", function (e) {
            e.metaKey = true;
        }).selectable({
            selected: function (event, ui) {
                check_Saveable();
                var el = $(ui.selected);
                switch (el.attr('id')) {
                    case "temp":
                        $('#<%=hf_Temp.ClientID%>').val("true");
                        break;
                    case "chills":
                        $('#<%=hf_Chills.ClientID%>').val("true");
                        break;
                    case "cough":
                        $('#<%=hf_Cough.ClientID%>').val("true");
                        break;
                    case "breath":
                        $('#<%=hf_Breath.ClientID%>').val("true");
                        break;
                    case "fatigue":
                        $('#<%=hf_Fatigue.ClientID%>').val("true");
                        break;
                    case "musclepain":
                        $('#<%=hf_MusclePain.ClientID%>').val("true");
                        break;
                    case "headache":
                        $('#<%=hf_Headache.ClientID%>').val("true");
                        break;
                    case "smellortaste":
                        $('#<%=hf_SmellOrTaste.ClientID%>').val("true");
                        break;
                    case "throat":
                        $('#<%=hf_Throat.ClientID%>').val("true");
                        break;
                    case "congestion":
                        $('#<%=hf_Congestion.ClientID%>').val("true");
                        break;                    
                    case "nausea":
                        $('#<%=hf_Nausea.ClientID%>').val("true");
                        break;
                    case "diarrhea":
                        $('#<%=hf_Diarrhea.ClientID%>').val("true");
                        break;
                    case "quarantined":
                        $('#<%=hf_Quarantined.ClientID%>').val("true");
                        break;
                }
            },
            unselected: function (event, ui) {
                check_Saveable();
                var el = $(ui.unselected);
                switch (el.attr('id')) {
                    case "temp":
                        $('#<%=hf_Temp.ClientID%>').val("false");
                        break;
                    case "chills":
                        $('#<%=hf_Chills.ClientID%>').val("false");
                        break;
                    case "cough":
                        $('#<%=hf_Cough.ClientID%>').val("false");
                        break;
                    case "breath":
                        $('#<%=hf_Breath.ClientID%>').val("false");
                        break;
                    case "fatigue":
                        $('#<%=hf_Fatigue.ClientID%>').val("false");
                        break;
                    case "musclepain":
                        $('#<%=hf_MusclePain.ClientID%>').val("false");
                        break;
                    case "headache":
                        $('#<%=hf_Headache.ClientID%>').val("false");
                        break;
                    case "smellortaste":
                        $('#<%=hf_SmellOrTaste.ClientID%>').val("false");
                        break;
                    case "throat":
                        $('#<%=hf_Throat.ClientID%>').val("false");
                        break;
                    case "congestion":
                        $('#<%=hf_Congestion.ClientID%>').val("false");
                        break;                    
                    case "nausea":
                        $('#<%=hf_Nausea.ClientID%>').val("false");
                        break;
                    case "diarrhea":
                        $('#<%=hf_Diarrhea.ClientID%>').val("false");
                        break;
                    case "quarantined":
                        $('#<%=hf_Quarantined.ClientID%>').val("false");
                        break;
                }
            },
            selecting: function (event, ui) {
                check_Saveable();
                var el = $(ui.selecting);
                if (el.attr('id') != "noSymptoms" && el.attr('id')!= 'quarantined') {
                    remove_NoSymptoms();
                }
                else if (el.attr('id') == "noSymptoms") {
                    remove_Selected();
                    remove_Selecting();
                }
            }
        });
    });

    function check_Saveable() {
        var selected_Symptoms = document.getElementsByClassName('symptom-selectable ui-selected');
        var selected_Campus = document.getElementsByClassName('campus-selectable ui-selected'); 
        if (selected_Symptoms.length > 0 && selected_Campus.length > 0) {
            $('#<%=hf_Saveable.ClientID%>').val("true");
        }
        else {
            $('#<%=hf_Saveable.ClientID%>').val("false");
        }
    }

    function remove_Selected() {
        var selected_Symptoms = document.querySelectorAll(".ui-selected");
        if (selected_Symptoms != null) {
            var symptoms_Length = selected_Symptoms.length;
            //console.log("There are " + symptoms_Length + " items to remove");
            for (var i = 0; i < symptoms_Length; i++) {
                var item = selected_Symptoms[i];
                console.log("Removing: " + item.id);
                if ((item.id != "offCampus") && (item.id != "onCampus") && (item.id != "quarantined")) {
                    item.classList.remove("ui-selected");
                }
                switch (item.id) {
                    case "temp":
                        $('#<%=hf_Temp.ClientID%>').val("false");
                        break;
                    case "chills":
                        $('#<%=hf_Chills.ClientID%>').val("false");
                        break;
                    case "cough":
                        $('#<%=hf_Cough.ClientID%>').val("false");
                        break;
                    case "breath":
                        $('#<%=hf_Breath.ClientID%>').val("false");
                        break;
                    case "fatigue":
                        $('#<%=hf_Fatigue.ClientID%>').val("false");
                        break;
                    case "musclepain":
                        $('#<%=hf_MusclePain.ClientID%>').val("false");
                        break;
                    case "headache":
                        $('#<%=hf_Headache.ClientID%>').val("false");
                        break;
                    case "smellortaste":
                        $('#<%=hf_SmellOrTaste.ClientID%>').val("false");
                        break;
                    case "throat":
                        $('#<%=hf_Throat.ClientID%>').val("false");
                        break;
                    case "congestion":
                        $('#<%=hf_Congestion.ClientID%>').val("false");
                        break;                    
                    case "nausea":
                        $('#<%=hf_Nausea.ClientID%>').val("false");
                        break;
                    case "diarrhea":
                        $('#<%=hf_Diarrhea.ClientID%>').val("false");
                        break;
                }
            }
        }
    }

    function remove_Selecting() {
        var selected_Symptoms = document.querySelectorAll(".ui-selecting");
        if (selected_Symptoms != null) {
            var symptoms_Length = selected_Symptoms.length;
            //console.log("There are " + symptoms_Length + " items to remove");
            for (var i = 0; i < symptoms_Length; i++) {
                var item = selected_Symptoms[i];
                console.log("Removing: " + item.id);
                if ((item.id != "offCampus") && (item.id != "onCampus") && (item.id != "quarantined") && (item.id != "noSymptoms")) {
                    item.classList.remove("ui-selecting");
                }
                switch (item.id) {
                    case "temp":
                        $('#<%=hf_Temp.ClientID%>').val("false");
                        break;
                    case "chills":
                        $('#<%=hf_Chills.ClientID%>').val("false");
                        break;
                    case "cough":
                        $('#<%=hf_Cough.ClientID%>').val("false");
                        break;
                    case "breath":
                        $('#<%=hf_Breath.ClientID%>').val("false");
                        break;
                    case "fatigue":
                        $('#<%=hf_Fatigue.ClientID%>').val("false");
                        break;
                    case "musclepain":
                        $('#<%=hf_MusclePain.ClientID%>').val("false");
                        break;
                    case "headache":
                        $('#<%=hf_Headache.ClientID%>').val("false");
                        break;
                    case "smellortaste":
                        $('#<%=hf_SmellOrTaste.ClientID%>').val("false");
                        break;
                    case "throat":
                        $('#<%=hf_Throat.ClientID%>').val("false");
                        break;
                    case "congestion":
                        $('#<%=hf_Congestion.ClientID%>').val("false");
                        break;                    
                    case "nausea":
                        $('#<%=hf_Nausea.ClientID%>').val("false");
                        break;
                    case "diarrhea":
                        $('#<%=hf_Diarrhea.ClientID%>').val("false");
                        break;
                }
            }
        }
    }

    function remove_NoSymptoms() {
        $("#noSymptoms").removeClass("ui-selected");
        $("#noSymptoms").removeClass("ui-selecting");
    }

    function SymptomsSelected() {
        var selected_Symptoms = document.getElementById("selectable").getElementsByClassName("ui-selected");
        //console.log(selected_Symptoms);
            if (selected_Symptoms != null) {
                var symptoms_Length = selected_Symptoms.length;
                if (symptoms_Length > 0)
                    return true;
                else
                    return false;
            }
    }
</script>
<asp:HiddenField ID="hf_Saveable" runat="server"/>
<asp:HiddenField ID="hf_Mask" runat="server" />
<asp:HiddenField ID="hf_OnCampus" runat="server" />
<asp:HiddenField ID="hf_Temp" runat="server"/>
<asp:HiddenField ID="hf_Chills" runat="server"/>
<asp:HiddenField ID="hf_Cough" runat="server"/>
<asp:HiddenField ID="hf_Breath" runat="server"/>
<asp:HiddenField ID="hf_Fatigue" runat="server"/>
<asp:HiddenField ID="hf_MusclePain" runat="server"/>
<asp:HiddenField ID="hf_Headache" runat="server"/>
<asp:HiddenField ID="hf_SmellOrTaste" runat="server"/>
<asp:HiddenField ID="hf_Throat" runat="server"/>
<asp:HiddenField ID="hf_Congestion" runat="server"/>
<asp:HiddenField ID="hf_Nausea" runat="server"/>
<asp:HiddenField ID="hf_Diarrhea" runat="server"/>
<asp:HiddenField ID="hf_Quarantined" runat="server"/>
<div id="div_Warning" class="alert alert-danger text-center" role="alert" runat="server">
    <h3>Please select at least one option for both questions below</h3>
</div>
<!--
<div>
    <h1 style="text-align: center;">Daily Covid Pass</h1>
<hr width="66%">
</div>
-->
<div id="main_Display" runat ="server" class="container">
    
    <!--
    <div class ="row">
        <div class="centre_Header bg-success">
            <h3><span class="bg-success border border-round">Are you coming to Campus?</span></h3>
        </div>
        <h3>Record your temperature, select your symptoms and submit this form as soon as you arrive on campus for work or before the start of your first class for the day.</h3> 
    </div>
    <div class ="row">
        <div class="centre_Header bg-warning">
            <h3><span class="bg-warning border border-round">Are you staying home?</span></h3>
        </div>
        <h3> You should have already made arrangements with your professor/supervisor. If you are not coming to campus then you do not need to report this information. You are encouraged to self-monitor for symptoms of COVID-19 and seek care as needed.</h3>
    </div>
        -->
    <div class ="row">
        <div class="centre_Header bg-success">
            <h3><span class="bg-success border border-round">Please fill out the form below</span></h3>
        </div>
        <h3><asp:literal ID="cntr_Header" runat="server">Everyone must take their temperature with a thermometer and complete this form every day. If you are coming to campus today, please complete this form before or as soon as you arrive on campus. If you are not coming to campus today please complete this form as soon as possible to ensure accurate reporting.</asp:literal></h3>
    </div>
    <div class ="border border-primary">
        <div class ="row">
            <h4>Name:</h4>
            <asp:Label ID="lbl_Name" runat="server" class="centre_Label"/>
        </div>
        <div class="row">
            <h4>Date:</h4>
            <asp:Label ID="lbl_Date" runat="server" class="centre_Label"/>
        </div>
        <div id="form_Div" runat="server">

            <div class ="row symptoms_Row">
                <h3>Please indicate symptoms that are new or different from what you ordinarily experience due to a preexisting condition; an injury or surgery; or recent physical activity. </h3>
                <ul id="selectable">
                    <li id="temp" class="ui-widget-content centre-row symptom-selectable">Temperature Over 100.4 Fahrenheit or 38.0 Celsius</li>
                    <li id="chills" class="ui-widget-content centre-row symptom-selectable">Chills</li>
                    <li id="cough" class="ui-widget-content centre-row symptom-selectable">Cough</li>
                    <li id="breath" class="ui-widget-content centre-row symptom-selectable">Shortness of Breath or Difficulty Breathing</li>
                    <li id="fatigue" class="ui-widget-content centre-row symptom-selectable">Fatigue</li>
                    <li id="musclepain" class="ui-widget-content centre-row symptom-selectable">Muscle or Body Aches</li>
                    <li id="headache" class="ui-widget-content centre-row symptom-selectable">Headache</li>
                    <li id="smellortaste" class="ui-widget-content centre-row symptom-selectable">New Loss of Smell or Taste</li>
                    <li id="throat" class="ui-widget-content centre-row symptom-selectable">Sore Throat</li>
                    <li id="congestion" class="ui-widget-content centre-row symptom-selectable">Congestion or Runny Nose</li>
                    <li id="nausea" class="ui-widget-content centre-row symptom-selectable">Nausea or Vomiting</li>
                    <li id="diarrhea" class="ui-widget-content centre-row symptom-selectable">Diarrhea</li>
                    <li id="quarantined" class="ui-widget-content centre-row symptom-selectable">Currently Quarantined/Isolated</li>
                    <li id="noSymptoms" class="ui-widget-content centre-row symptom-selectable">I Have No Symptoms</li>
                    <!--
                    <li id="otherSymptoms" class="ui-widget-content  centre-row">Other Symptoms</li>
                    -->
                </ul>
            </div>
            <!--
            <div id="div_OtherSymptoms" class="row">
                <h3>Other symptoms?</h3>
                <asp:TextBox ID="tbx_Symptoms" TextMode="MultiLine" runat="server" class="centre_TextBox"/>
            </div>
            -->
            <div class="row">
                <h3>Will you be on Campus Today?</h3>
                <ol id="centre-selectable">
                        <li id="onCampus" class="ui-widget-content centre-row campus-selectable">I will be on campus</li>
                        <li id="offCampus" class="ui-widget-content centre-row campus-selectable">I will not be on campus</li>
                    </ol>
            </div>
            <div class="row">
                <asp:Button ID="Submit" Text="Submit Form" runat="server" class="btn btn-primary cntr-btn" OnClick="Submit_Click"/>
            </div>
        </div>
    </div>
    
</div>
<div id="completion_Display" class ="container text-center" runat="server">
    <div class="row">
        <div class ="col-md-12 panel panel-default">
            <h2><asp:Label ID="lbl_Clearance" runat="server"></asp:Label></h2>
            <div id="div_Image" runat="server"></div>
        </div>
    </div>
    <div class="row">
        <div class ="col-md-6 panel panel-default">
            <h3>Name:</h3>
            <h2><asp:Label ID="lbl_completion_Name" runat="server"></asp:Label></h2>
        </div>
        <div class ="col-md-6 panel panel-default">
            <h3>Date:</h3>
            <h2><asp:Label ID="lbl_completion_Date" runat="server"></asp:Label></h2>
        </div>
    </div>
    <div class="row">
        <div class ="col-md-12 panel panel-default">
            <h3>Reported Symptoms</h3>
            <h2><asp:Label ID="lbl_Symptoms" runat="server"></asp:Label></h2>
        </div>
    </div>
    <div class ="row">
        <div class ="col-md-12 ui-tabs-panel panel panel-default">
            <h3>If you made a mistake in filling out today's form, click below to resubmit</h3>
            <asp:Button ID="ResubmitButton" Text="Resubmit Form" runat="server" class="btn btn-primary cntr-btn" OnClick="ResubmitButton_Click"/>
        </div>
    </div>
    <div id="Reporting_Row" runat="server" class="row">
            <div class="col-md-6 panel panel-default">
                <h3>Covid Reporting</h3>
                <a class="btn btn-primary cntr-btn" href="/ICS/Campus_Resources/Forms__Docs/Daily_COVID_Pass/COVID-19_Reporting.jnz">View COVID-19 Symptom Reports</a>
            </div>
            <div class="col-md-6 panel panel-default">
                <h3>Covid Testing Reports</h3>
                <a class="btn btn-primary cntr-btn" href="/ICS/Campus_Resources/Forms__Docs/Daily_COVID_Pass/COVID-19_Testing.jnz">Submit New Test or View Old Tests</a>
            </div>
        </div>
</div>

<div id ="success_Display" class="container" runat="server">
    <h2>Thank you for submitting your Self Assesment for COVID-19 today, please return tomorrow for another self reporting form.</h2>
</div>

