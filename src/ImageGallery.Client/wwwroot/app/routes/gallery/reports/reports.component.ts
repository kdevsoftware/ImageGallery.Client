import { Component, OnInit } from '@angular/core';
import { TitleService } from '../../../services/title.service';
import { GalleryService } from '../../../gallery.service';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss'],
  providers: [GalleryService]
})
export class ReportsComponent implements OnInit {

  bookReports: any[] = [];
  referenceReports: any[] = [];
  catalogReports: any[] = [];
  selectedReport: any;
  reportsUrl = "https://api-reports.navigatorglass.com/api/Reports"

  constructor(
    private readonly galleryService: GalleryService,
    private spinnerService: NgxLoadingSpinnerService,
    private titleService: TitleService) { }

  ngOnInit() {
    this.titleService.set('Reports');
    this.getReports();
  }

  getReports() {
    this.spinnerService.show();

    this.galleryService.getReports('books')
      .subscribe((res: any[]) => {
        this.bookReports = res;
        this.spinnerService.hide();
      });
    this.galleryService.getReports('catalogs')
      .subscribe((res: any[]) => {
        this.catalogReports = res;
        this.spinnerService.hide();
      });
    this.galleryService.getReports('references')
      .subscribe((res: any[]) => {
        this.referenceReports = res;
        this.spinnerService.hide();
      });
  }

  onSelectBook() {
    var link = `${this.reportsUrl}?ReferenceId=${this.selectedReport.key}&ReportType=book`;
    window.open(link, "_blank");
  }

  onSelectCatalog() {
    var link = `${this.reportsUrl}?Catalog=${this.selectedReport.key}&ReportType=catalog`;
    window.open(link, "_blank");
  }

  onSelectReference() {
    var link = `${this.reportsUrl}?ReferenceId=${this.selectedReport.key}&ReportType=reference`;
    window.open(link, "_blank");
  }

}
