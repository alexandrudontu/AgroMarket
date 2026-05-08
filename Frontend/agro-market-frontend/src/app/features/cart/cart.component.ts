import { Component, OnInit } from '@angular/core';
import { CartService } from './cart.service';
import { OrdersService } from '.././orders/orders.service';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  imports: [CommonModule]
})
export class CartComponent implements OnInit {

  cart: any;

  constructor(
    private cartService: CartService,
    private orderService: OrdersService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.cartService.getCart().subscribe((res: any) => {
      this.cart = res;
      this.cdr.detectChanges();
    });
  }

  remove(productId: number) {
    this.cartService.remove(productId).subscribe(() => {
      this.loadCart();
      this.cdr.detectChanges();
    });
  }

  clear() {
    this.cartService.clear().subscribe(() => {
      this.loadCart();
      this.cdr.detectChanges();
    });
  }

  checkout() {
    this.orderService.checkout().subscribe({
      next: (res: any) => {
        alert('Order placed! ID: ' + res.orderId);
        this.loadCart();
        this.cdr.detectChanges();
      },
      error: () => {
        alert('Checkout failed');
      }
    });
  }
}
