import { createRouter, createWebHistory } from 'vue-router'
import SalaryComposition from '@/views/salary-composition/SalaryComposition.vue'
import SalaryCompositionSystem from '@/views/salary-composition/SalaryCompositionSystem.vue'
import News from '@/views/news/News.vue'
import NotFound from '@/views/not-found/NotFound.vue'
import TheLayout from '@/layout/TheLayout.vue'

const routes = [
  {
    path: '/',
    component: TheLayout,
    children: [
      {
        path: '',
        redirect: '/salary-composition',
      },
      {
        path: 'salary-composition',
        name: 'salary-composition',
        component: SalaryComposition,
        children: [
          {
            path: 'system-category',
            name: 'SalaryCompositionSystem',
            component: SalaryCompositionSystem,
          },
        ],
      },
      {
        path: 'recruitment-news',
        name: 'recruitment-news',
        component: News,
      },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: NotFound,
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
