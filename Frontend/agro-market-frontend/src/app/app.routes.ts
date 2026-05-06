import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home'; 
import { ProductDetailsComponent } from './features/products/product-details/product-details';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { CartComponent } from './features/cart/cart.component';
import { OrdersComponent } from './features/orders/orders.component';
import { FarmerDashboardComponent } from './features/farmer/dashboard/dashboard';
import { FarmersComponent } from './features/admin/farmers/farmers';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
import { CategoriesListComponent } from './features/categories/categories-list';
import { CategoryProductsComponent } from './features/categories/category-products/category-products';

export const routes: Routes = [
        { path: '', component: HomeComponent },
        { path: 'login', component: LoginComponent },
        { path: 'register', component: RegisterComponent },
        { path: 'cart', 
          component: CartComponent,
          canActivate: [authGuard] },
        {
          path: 'categories',
          component: CategoriesListComponent
        },
        { path: 'categories/:id', component: CategoryProductsComponent },
        { path: 'products/:id', component: ProductDetailsComponent },
        {
          path: 'farmer',
          component: FarmerDashboardComponent,
          canActivate: [authGuard, roleGuard],
          data: { role: 'Farmer' }
        },
        {
          path: 'orders',
          component: OrdersComponent,
          canActivate: [authGuard]
        },
        {
          path: 'admin/farmers',
          component: FarmersComponent,
          canActivate: [authGuard, roleGuard],
          data: { role: 'Admin' }
        }

];
