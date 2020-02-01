var addButton = $("button#add_row");
var removeButton = $("button#remove_row");

addButton.on("click", function() {
   var templateRow = $("tr#template_row");
   
   var currentCount = $("table tbody tr").length;
   if (currentCount >= 51) {
      alert("Es sind nicht mehr als 50 Artikel pro Registrierung möglich. Bitte registrieren Sie sich ein weiteres Mal, um mehr Artikel anzugeben.")
      return;
   }
   
   var newRow = templateRow.clone();
   newRow.prop("id", "");
   newRow.css("display", "table-row");
   
   newRow.find("td:nth-of-type(1)").text(currentCount);
   newRow.find("td:nth-of-type(2) input").prop("name", "Articles[" + (currentCount - 1) + "].Name");
   newRow.find("td:nth-of-type(3) input").prop("name", "Articles[" + (currentCount - 1) + "].Size");
   newRow.find("td:nth-of-type(4) input").prop("name", "Articles[" + (currentCount - 1) + "].Price");
   
   $("table tbody").append(newRow);
   newRow.find("td:nth-of-type(2) input").focus();
});

removeButton.on("click", function() {
   $("table tbody tr:last-of-type").remove();
});

$(document).ready(function() {
   if ($("table tbody tr").length <= 1) {
      addButton.click();
      $("body input#Name").focus();
   }
});