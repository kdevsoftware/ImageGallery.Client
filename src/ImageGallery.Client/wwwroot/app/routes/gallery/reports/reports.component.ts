import { Component, OnInit } from '@angular/core';
import { TitleService } from '../../../services/title.service';
import { GalleryService } from '../../../gallery.service';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { combineLatest } from 'rxjs/operators';
import { zip } from 'rxjs';

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

    const books$ = this.galleryService.getReports('books');
    const catalogs$ = this.galleryService.getReports('catalogs')
    const references$ = this.galleryService.getReports('references');

    zip(books$, catalogs$, references$,
      (books, catalogs, references) => ({ books, catalogs, references }))
      .subscribe((pair: any) => {
        pair.books.forEach(book => {
          book.downloadUrl = `${this.reportsUrl}?ReferenceId=${book.key}&ReportType=book`;
        });
        this.bookReports = pair.books;

        pair.catalogs.forEach(catalog => {
          catalog.downloadUrl = `${this.reportsUrl}?Catalog=${catalog.key}&ReportType=catalog`;
        });
        this.catalogReports = pair.catalogs;

        pair.references.forEach(reference => {
          reference.downloadUrl = `${this.reportsUrl}?ReferenceId=${reference.key}&ReportType=reference`;
        });

        this.referenceReports = pair.references;
        this.spinnerService.hide();
      })
  }

  // onSelectBook() {
  //   var link = 
  //   window.open(link, "_blank");
  // }

  // onSelectCatalog() {
  //   var link = `${this.reportsUrl}?Catalog=${this.selectedReport.key}&ReportType=catalog`;
  //   window.open(link, "_blank");
  // }

  // onSelectReference() {
  //   var link = `${this.reportsUrl}?ReferenceId=${this.selectedReport.key}&ReportType=reference`;
  //   window.open(link, "_blank");
  // }

}
