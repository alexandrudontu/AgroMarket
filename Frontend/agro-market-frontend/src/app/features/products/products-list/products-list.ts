import { Component } from '@angular/core';
import { ProductsService } from '../products.service';
import { CartService } from '../../cart/cart.service';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-products-list',
  imports: [CommonModule],
  templateUrl: './products-list.html',
  styleUrl: './products-list.css',
})
export class ProductsList {

  products: any[] = [];

  constructor(private productService: ProductsService,
  private cartService: CartService) {}

  ngOnInit() {
    this.productService.getAll().subscribe((res: any) => {
      this.products = res;
    });
  }

  addToCart(productId: number) {
    this.cartService.addToCart({
      productId: productId,
      quantity: 1
    }).subscribe();
  }
}
