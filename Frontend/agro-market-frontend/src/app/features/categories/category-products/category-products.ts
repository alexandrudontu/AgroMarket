import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductsService } from '../../products/products.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-category-products',
  standalone: true,
  templateUrl: './category-products.html',
  imports: [RouterModule, CommonModule, FormsModule]
})
export class CategoryProductsComponent implements OnInit {

  products: any[] = [];

  categoryId!: number;

  minPrice?: number;
  maxPrice?: number;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService
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
    });
  }

  applyFilters() {
    this.loadProducts();
  }
}