import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProductsService } from '../products.service';
import { CartService } from '../../cart/cart.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.html',
  standalone: true,
  imports: [CommonModule]
})
export class ProductDetailsComponent implements OnInit {

  product: any;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    private cartService: CartService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id') ?? '0');

    this.productService.getById(id).subscribe((res: any) => {
      this.product = res;
    });
  }

  addToCart() {
    this.cartService.addToCart({
      productId: this.product.id,
      quantity: 1
    }).subscribe(() => alert('Added to cart'));
  }
}
