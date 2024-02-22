import { jsPDF } from "jspdf"
var font = 'undefined';
var callAddFont = function () {
this.addFileToVFS('Amiri -normal.ttf', font);
this.addFont('Amiri -normal.ttf', 'Amiri ', 'normal');
};
jsPDF.API.events.push(['addFonts', callAddFont])
