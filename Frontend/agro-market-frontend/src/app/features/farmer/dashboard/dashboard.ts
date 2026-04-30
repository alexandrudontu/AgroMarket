import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../../products/products.service';
import { OrdersService } from '../../orders/orders.service';

@Component({
  selector: 'app-farmer-dashboard',
  templateUrl: './dashboard.html'
})
export class FarmerDashboardComponent implements OnInit {

  products: any[] = [];
  orders: any[] = [];

  constructor(
    private productService: ProductsService,
    private orderService: OrdersService
  ) {}

  ngOnInit() {
    this.loadProducts();
    this.loadOrders();
  }

  loadProducts() {
    this.productService.getMyProducts().subscribe((res: any) => {
      this.products = res;
    });
  }

  loadOrders() {
    this.orderService.getFarmerOrders().subscribe((res: any) => {
      this.orders = res;
    });
  }
}
