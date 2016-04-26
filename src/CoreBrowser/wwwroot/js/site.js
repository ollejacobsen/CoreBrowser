$(document).ready(function () {
	$("table.sortable").stupidtable();

	$(document).on("click", "table tr", function (evt) {
		var url = $(this).attr("data-url"),
			type = $(this).attr("data-type"),
			links = $(this).find("a");

		if (typeof url === "undefined" || typeof type === "undefined")
			return;

		evt.preventDefault();

		if (type === "folder" || type === "parent-folder") {
			window.location.href = url;
		}
		else if (type === "image") {
			links.first().ekkoLightbox({
				always_show_close: false,
				loadingMessage: '<div class="text-center"><i class="fa fa-circle-o-notch fa-spin fa-3x fa-fw margin-bottom"></i><span class="sr-only">Loading...</span></div>'
			});
		} else {
			window.location.href = url;
		}
	});
});