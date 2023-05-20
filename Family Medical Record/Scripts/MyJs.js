function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imagePreview').css('background-image', 'url(' + e.target.result + ')');
            $('#imagePreview').fadeIn(650);
        }
        
        reader.readAsDataURL(input.files[0]);

        console.log("Done...");
    }
}

$("#imageUpload").change(function () {
    readURL(this);
});


const dropArea = document.querySelector(".drop_box"),
  button = dropArea.querySelector("button"),
  dragText = dropArea.querySelector("header"),
  input = dropArea.querySelector("input");
let file;
var filename;

button.onclick = () => {
  input.click();
};

input.addEventListener("change", function (e) {
  var fileName = e.target.files[0].name;
  let filedata = `
    <div class="FileUpload">
    <h4>${fileName}</h4>
    <button class="btn">Upload</button>
    </div>`;
  dropArea.innerHTML = filedata;
});

$(function () {
    $("div[data-navigation='true']").find("li").children("a").each(function () {
        if ($(this).attr("href") === window.location.pathname) {
            $(this).parent().addClass("active");
        }
    });
});