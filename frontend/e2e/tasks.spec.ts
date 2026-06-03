import { test, expect } from '@playwright/test'

test.describe('Tasks', () => {
  let uniqueId: string

  test.beforeEach(async ({ page }) => {
    uniqueId = Date.now().toString(36)
    await page.goto('/login')
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
    await page.getByRole('link', { name: /tasks/i }).click()
    await expect(page).toHaveURL('/tasks')
  })

  test('should display tasks page with create form', async ({ page }) => {
    await expect(page.getByText('Create New Task')).toBeVisible()
    await expect(page.getByPlaceholder('Task title')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Add Task' })).toBeVisible()
  })

  test('should create a new task', async ({ page }) => {
    const taskName = `Buy groceries ${uniqueId}`
    await page.getByPlaceholder('Task title').fill(taskName)
    await page.locator('form select').first().selectOption({ label: 'demo' })
    await page.getByRole('button', { name: 'Add Task' }).click()

    await expect(page.getByText(taskName)).toBeVisible({ timeout: 5000 })
  })

  test('should complete a task', async ({ page }) => {
    const taskName = `Task to complete ${uniqueId}`
    await page.getByPlaceholder('Task title').fill(taskName)
    await page.locator('form select').first().selectOption({ label: 'demo' })
    await page.getByRole('button', { name: 'Add Task' }).click()
    await expect(page.getByText(taskName)).toBeVisible({ timeout: 5000 })

    // Click the complete button on the newly created task
    const taskEl = page.getByTestId(/task-/).filter({ hasText: taskName })
    await taskEl.getByRole('button', { name: 'Complete task' }).click()

    // Should show green checkmark
    await expect(taskEl.locator('.text-green-500')).toBeVisible({ timeout: 5000 })
  })

  test('should open comments panel for a task', async ({ page }) => {
    const taskName = `Task with comments ${uniqueId}`
    await page.getByPlaceholder('Task title').fill(taskName)
    await page.locator('form select').first().selectOption({ label: 'demo' })
    await page.getByRole('button', { name: 'Add Task' }).click()
    await expect(page.getByText(taskName)).toBeVisible({ timeout: 5000 })

    // Click the comments icon
    const taskEl = page.getByTestId(/task-/).filter({ hasText: taskName })
    await taskEl.getByRole('button', { name: 'View comments' }).click()

    // Comments panel should appear
    await expect(page.getByRole('heading', { name: 'Comments' })).toBeVisible()
  })

  test('should add a comment to a task', async ({ page }) => {
    const taskName = `Commentable task ${uniqueId}`
    await page.getByPlaceholder('Task title').fill(taskName)
    await page.locator('form select').first().selectOption({ label: 'demo' })
    await page.getByRole('button', { name: 'Add Task' }).click()
    await expect(page.getByText(taskName)).toBeVisible({ timeout: 5000 })

    // Open comments
    const taskEl = page.getByTestId(/task-/).filter({ hasText: taskName })
    await taskEl.getByRole('button', { name: 'View comments' }).click()
    await expect(page.getByRole('heading', { name: 'Comments' })).toBeVisible()

    // Add a comment
    await page.getByPlaceholder('Write a comment...').fill('This is a test comment')
    await page.locator('form').last().getByRole('button').click()

    // Comment should appear
    await expect(page.getByText('This is a test comment')).toBeVisible({ timeout: 5000 })
  })
})
