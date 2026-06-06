import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../products/products.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs/internal/Subject';
import { debounceTime, switchMap } from 'rxjs/operators';
import { ChangeDetectorRef } from '@angular/core';
import { ElementRef, HostListener } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
})

export class HomeComponent implements OnInit {

  products: any[] = [];
  searchResults: any[] = [];
  search = '';
  apiUrl = environment.apiUrl;

  private searchSubject = new Subject<string>();

  constructor(private productService: ProductsService, private cdr: ChangeDetectorRef, private elRef: ElementRef) {}

  ngOnInit() {

    this.searchSubject.pipe(
      debounceTime(300),
      switchMap(term =>
        this.productService.getAll({ search: term })
      )
    ).subscribe(res => {
      this.searchResults = res.slice(0, 5);
      this.cdr.detectChanges();
    });

    this.loadRecommended();
  }

  loadRecommended() {
    this.productService.getAll().subscribe(res => {
      this.products = res;
      this.cdr.detectChanges();
    });
  }

  onSearchChange() {
    if (!this.search) {
      this.searchResults = [];
      return;
    }

    this.searchSubject.next(this.search);
  }

  selectResult() {
    this.search = '';
    this.searchResults = [];
    this.cdr.detectChanges();
  }

  @HostListener('document:click', ['$event']) onClickOutside(event: MouseEvent) {

    if (!this.elRef.nativeElement.contains(event.target)) {

      this.searchResults = [];

      this.cdr.detectChanges();
    }
  }

  getImageUrl(imageUrl: string): string {
    if (!imageUrl) {
      return 'https://placehold.co/400x300?text=No+Image';
    }
  
    if (imageUrl.startsWith('http')) {
      return imageUrl;
    }
  
    return this.apiUrl + imageUrl;
  }
}