import { Component, OnInit } from '@angular/core';
import { CartService } from './cart.service';
import { OrdersService } from '.././orders/orders.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  imports: [CommonModule]
})
export class CartComponent implements OnInit {

  cart: any;

  constructor(
    private cartService: CartService,
    private orderService: OrdersService
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.cartService.getCart().subscribe((res: any) => {
      this.cart = res;
    });
  }

  remove(productId: number) {
    this.cartService.remove(productId).subscribe(() => {
      this.loadCart();
    });
  }

  clear() {
    this.cartService.clear().subscribe(() => {
      this.loadCart();
    });
  }

  checkout() {
    this.orderService.checkout().subscribe({
      next: (res: any) => {
        alert('Order placed! ID: ' + res.orderId);
        this.loadCart();
      },
      error: () => {
        alert('Checkout failed');
      }
    });
  }
}
