import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductsService } from '../../products/products.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-category-products',
  standalone: true,
  templateUrl: './category-products.html',
  styleUrls: ['./category-products.css'],
  imports: [RouterModule, CommonModule, FormsModule]
})
export class CategoryProductsComponent implements OnInit {

  products: any[] = [];

  categoryId!: number;

  minPrice?: number;
  maxPrice?: number;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.categoryId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getAll({
      categoryId: this.categoryId,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice
    }).subscribe(res => {
      this.products = res;
      this.cdr.detectChanges();
    });
  }

  applyFilters() {
    if (this.minPrice != null && this.minPrice < 0) {
      this.minPrice = 0;
    }

    if (this.maxPrice != null && this.maxPrice < 0) {
      this.maxPrice = 0;
    }

    if (
      this.minPrice != null &&
      this.maxPrice != null &&
      this.minPrice > this.maxPrice
    ) {
      return;
    }

    this.loadProducts();
  }
}